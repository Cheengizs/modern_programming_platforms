using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using CheengizsTestLib.Attributes;
using CheengizsTestLib.Exceptions;

namespace TestRunner;

public class RunnerConfig
{
    public bool RunInParallel { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
}

public class TestStatistics
{
    private int _total;
    private int _passed;
    private int _failed;

    public void AddTotal() => Interlocked.Increment(ref _total);
    public void AddPassed() => Interlocked.Increment(ref _passed);
    public void AddFailed() => Interlocked.Increment(ref _failed);

    public void PrintSummary(TimeSpan duration)
    {
        Console.WriteLine("\n--------------------------------------------------");
        Console.WriteLine($"Total: {_total}, Success: {_passed}, Failed: {_failed}");
        Console.WriteLine($"Time elapsed: {duration.TotalSeconds:F2} sec");
    }
}

public static class ConsoleUi
{
    private static readonly object _consoleLock = new();

    public static void PrintError(string message) => PrintColored(ConsoleColor.Red, message);
    public static void PrintSuccess(string message) => PrintColored(ConsoleColor.Green, message);

    public static void PrintHeader(string message)
    {
        lock (_consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
        }
    }

    public static void PrintTestResult(string testName, string resultMessage, bool isSuccess)
    {
        lock (_consoleLock)
        {
            Console.Write($"Test: {testName} -> ");
            if (isSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(resultMessage);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(resultMessage);
            }

            Console.ResetColor();
        }
    }

    private static void PrintColored(ConsoleColor color, string message)
    {
        lock (_consoleLock)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // if (args.Length == 0)
        // {
        //     ConsoleUi.PrintError("Error: No DLL path provided.");
        //     return;
        // }


        // string assemblyPath = args[0];
        string assemblyPath =
            @"D:\study\6semester\mpp\lab1\CheengizsRPG.Tests\bin\Debug\net10.0\CheengizsRPG.Tests.dll";
        
        var config = ParseArgs(args);

        if (!File.Exists(assemblyPath))
        {
            ConsoleUi.PrintError($"Error: File not found -> {assemblyPath}");
            return;
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();

            ConsoleUi.PrintHeader(
                $"Running mode: {(config.RunInParallel ? $"PARALLEL (Max threads: {config.MaxDegreeOfParallelism})" : "SEQUENTIAL")}");

            RunTestsFromAssembly(assemblyPath, config);

            stopwatch.Stop();
        }
        catch (Exception ex)
        {
            ConsoleUi.PrintError($"Critical infrastructure error: {ex.Message}");
        }
    }

    private static RunnerConfig ParseArgs(string[] args)
    {
        var config = new RunnerConfig();
        if (args.Contains("--seq"))
        {
            config.RunInParallel = false;
        }

        int threadIndex = Array.IndexOf(args, "--threads");
        if (threadIndex > -1 && threadIndex + 1 < args.Length)
        {
            if (int.TryParse(args[threadIndex + 1], out int threads))
            {
                config.MaxDegreeOfParallelism = threads;
            }
        }

        return config;
    }

    private static void RunTestsFromAssembly(string path, RunnerConfig config)
    {
        var assembly = Assembly.LoadFrom(path);
        var statistics = new TestStatistics();
        var stopwatch = Stopwatch.StartNew();

        var testTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null)
            .ToList();

        foreach (var type in testTypes)
        {
            RunTestClass(type, config, statistics);
        }

        stopwatch.Stop();
        statistics.PrintSummary(stopwatch.Elapsed);
    }

    private static void RunTestClass(Type testType, RunnerConfig config, TestStatistics stats)
    {
        ConsoleUi.PrintHeader($"Scanning Class: {testType.Name}...");

        var initMethod = GetMethodByAttribute<TestInitial>(testType);
        var endMethod = GetMethodByAttribute<TestEnd>(testType);

        var testMethods = testType.GetMethods()
            .Where(m => m.GetCustomAttribute<TestMethodAttribute>() != null)
            .OrderBy(m => m.GetCustomAttribute<TestMethodAttribute>()?.Priority ?? 0)
            .ToList();

        var runnables = new List<(MethodInfo Method, object?[]? Params)>();

        foreach (var method in testMethods)
        {
            var dataAttributes = method.GetCustomAttributes<DataAttribute>().ToList();
            var scenarios = dataAttributes.Any()
                ? dataAttributes.Select(attr => attr.Data).ToList()
                : new List<object?[]> { null };

            foreach (var parameters in scenarios)
            {
                runnables.Add((method, parameters));
            }
        }

        if (config.RunInParallel)
        {
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = config.MaxDegreeOfParallelism };

            Parallel.ForEach(runnables, parallelOptions,
                item => { ExecuteSingleTest(testType, item.Method, initMethod, endMethod, item.Params, stats); });
        }
        else
        {
            foreach (var item in runnables)
            {
                ExecuteSingleTest(testType, item.Method, initMethod, endMethod, item.Params, stats);
            }
        }
    }

    private static void ExecuteSingleTest(
        Type testClassType,
        MethodInfo testMethod,
        MethodInfo? initMethod,
        MethodInfo? endMethod,
        object?[]? parameters,
        TestStatistics stats)
    {
        stats.AddTotal();

        object? testInstance = Activator.CreateInstance(testClassType);

        string paramsInfo = parameters != null ? $"({string.Join(", ", parameters)})" : "";
        string testName = $"{testMethod.Name}{paramsInfo}";

        var timeoutAttr = testMethod.GetCustomAttribute<TestTimeoutAttribute>();
        int timeout = timeoutAttr?.TimeoutMilliseconds ?? Timeout.Infinite;

        try
        {
            Task testTask = Task.Run(() =>
            {
                initMethod?.Invoke(testInstance, null);

                var result = testMethod.Invoke(testInstance, parameters);

                if (result is Task t) t.Wait();
            });

            bool completedInTime = testTask.Wait(timeout);

            if (!completedInTime)
            {
                throw new TimeoutException($"Test exceeded timeout of {timeout}ms");
            }

            if (testTask.Exception != null)
            {
                throw testTask.Exception;
            }

            stats.AddPassed();
            ConsoleUi.PrintTestResult(testName, "Success", true);
        }
        catch (Exception ex)
        {
            stats.AddFailed();

            Exception realEx = ex;
            if (realEx is AggregateException ae) realEx = ae.InnerException ?? realEx;
            if (realEx is TargetInvocationException tie) realEx = tie.InnerException ?? realEx;

            string msg = realEx is AssertFailedException
                ? $"Failed: {realEx.Message}"
                : $"Error: {realEx.GetType().Name} - {realEx.Message}";

            ConsoleUi.PrintTestResult(testName, msg, false);
        }
        finally
        {
            try
            {
                endMethod?.Invoke(testInstance, null);
            }
            catch
            {
                throw;
                // i dont have end methods, so, i dont care about exception handling))}
            }
        }
    }

    private static MethodInfo? GetMethodByAttribute<TAttribute>(Type type) where TAttribute : Attribute
    {
        return type.GetMethods().FirstOrDefault(m => m.GetCustomAttribute<TAttribute>() != null);
    }
}