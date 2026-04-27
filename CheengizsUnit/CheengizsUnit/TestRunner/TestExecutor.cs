using System.Diagnostics;
using System.Reflection;
using TestRunner.Threading;
using UnitTestLib.Attributes;
using UnitTestLib.Exceptions;

namespace TestRunner;

public class TestExecutor
{
    private int _totalPassed;
    private int _totalFailed;

    private class TestCase
    {
        public MethodInfo Method { get; set; } = null!;
        public object[]? Parameters { get; set; }
    }

    public void RunTests(string assemblyPath, InputHandler.ExecutionMode mode, int maxDegree, Func<MethodInfo, bool>? testFilter = null)
    {
        if (!File.Exists(assemblyPath)) return;
        Assembly testsAssembly = Assembly.LoadFrom(assemblyPath);

        var testClasses = testsAssembly.GetTypes().Where(t => t.IsDefined(typeof(TestClassAttribute), false));

        _totalPassed = 0;
        _totalFailed = 0;
        var totalSw = Stopwatch.StartNew(); 

        foreach (var type in testClasses)
        {
            var classAttr = type.GetCustomAttribute<TestClassAttribute>();
            if (classAttr?.Ignore == true) continue;

            TestReporter.PrintHeader($"Running [{mode}] tests in: {type.Name}");

            object? sharedContext = classAttr?.ContextType != null 
                ? Activator.CreateInstance(classAttr.ContextType) : null;
            
            var testMethods = type.GetMethods()
                .Where(m => m.IsDefined(typeof(TestMethodAttribute), false))
                .Where(m => testFilter == null || testFilter(m))
                .ToList();

            var testCasesToRun = new List<TestCase>();

            foreach (var method in testMethods)
            {
                var dataAttr = method.GetCustomAttribute<MethodDataAttribute>();
                var dynamicDataAttr = method.GetCustomAttribute<DynamicDataAttribute>();

                if (dataAttr != null)
                {
                    var dataList = typeof(MethodDataAttribute).GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(dataAttr) as IEnumerable<object>;
                    if (dataList != null) foreach (var item in dataList) testCasesToRun.Add(new TestCase { Method = method, Parameters = new object[] { item } });
                }
                else if (dynamicDataAttr != null)
                {
                    var dataMethod = type.GetMethod(dynamicDataAttr.MethodName, BindingFlags.Public | BindingFlags.Static);
                    if (dataMethod != null)
                    {
                        var data = (IEnumerable<object[]>)dataMethod.Invoke(null, null)!;
                        foreach (var item in data) testCasesToRun.Add(new TestCase { Method = method, Parameters = item });
                    }
                }
                else testCasesToRun.Add(new TestCase { Method = method, Parameters = null });
            }

            if (mode == InputHandler.ExecutionMode.Sync)
            {
                foreach (var tc in testCasesToRun) ExecuteTestCase(tc, type, sharedContext);
            }
            else if (mode == InputHandler.ExecutionMode.ParallelForEach)
            {
                Parallel.ForEach(testCasesToRun, new ParallelOptions { MaxDegreeOfParallelism = maxDegree }, tc => ExecuteTestCase(tc, type, sharedContext));
            }
            else if (mode == InputHandler.ExecutionMode.CustomThreadPool)
            {
                RunWithCustomPoolAndLoadSimulation(testCasesToRun, type, sharedContext, maxDegree);
            }
        }

        totalSw.Stop(); 
        TestReporter.PrintSummary(_totalPassed, _totalFailed, totalSw.ElapsedMilliseconds);
    }

    private void RunWithCustomPoolAndLoadSimulation(List<TestCase> testCases, Type type, object? sharedContext, int maxThreads)
    {
        if (testCases.Count == 0) return;

        using var countdown = new CountdownEvent(testCases.Count);
        using var pool = new CustomThreadPool(minThreads: 2, maxThreads: maxThreads);

        pool.ThreadCreated += id => TestReporter.PrintError($"[Event] Thread Created: {id}");
        pool.ThreadDestroyed += id => TestReporter.PrintError($"[Event] Thread Destroyed: {id}");
        pool.TaskEnqueued += count => Console.WriteLine($"[Event] Task Enqueued. Queue Size: {count}");
        pool.TaskCompleted += id => Console.WriteLine($"[Event] Task Completed by Thread: {id}");
        
        Action CreateTask(TestCase tc) => () =>
        {
            ExecuteTestCase(tc, type, sharedContext);
            countdown.Signal(); 
        };

        TestReporter.PrintHeader(">>> SIMULATING UNEVEN LOAD SCENARIO <<<");

        int total = testCases.Count;
        int p1 = total / 3;
        int p2 = total * 2 / 3;

        Console.WriteLine("\n[SCENARIO] 1. Peak Load Injection...");
        for (int i = 0; i < p1; i++) pool.EnqueueTask(CreateTask(testCases[i]));

        Console.WriteLine("\n[SCENARIO] 2. Idle Period (3 seconds)... Watch threads scale down.");
        Thread.Sleep(3000);

        Console.WriteLine("\n[SCENARIO] 3. Single Injections...");
        for (int i = p1; i < p2; i++)
        {
            pool.EnqueueTask(CreateTask(testCases[i]));
            Thread.Sleep(200); 
        }

        Console.WriteLine("\n[SCENARIO] 4. Second Peak Load...");
        for (int i = p2; i < total; i++) pool.EnqueueTask(CreateTask(testCases[i]));

        countdown.Wait();
    }

    private void ExecuteTestCase(TestCase tc, Type type, object? sharedContext)
    {
        var instance = Activator.CreateInstance(type);
        var prop = instance?.GetType().GetProperty("Context");
        prop?.SetValue(instance, sharedContext);

        int p = 0, f = 0;
        RunSingleTest(instance, tc.Method, type, tc.Parameters, sharedContext, ref p, ref f);
        Interlocked.Add(ref _totalPassed, p);
        Interlocked.Add(ref _totalFailed, f);
    }

    private void RunSingleTest(object? instance, MethodInfo method, Type type, object[]? parameters, object? context, ref int passed, ref int failed)
    {
        string paramDisplay = parameters != null && parameters.Length > 0 ? parameters[0]?.ToString() ?? "null" : "";
        string testDisplayName = string.IsNullOrEmpty(paramDisplay) ? $"{method.Name}()" : $"{method.Name}({paramDisplay})";

        var init = type.GetMethods().FirstOrDefault(m => m.IsDefined(typeof(TestInitializeAttribute), false));
        var cleanup = type.GetMethods().FirstOrDefault(m => m.IsDefined(typeof(TestCleanupAttribute), false));

        var testMethodAttr = method.GetCustomAttribute<TestMethodAttribute>();
        int timeoutMs = testMethodAttr?.Timeout ?? 0;

        var sw = new Stopwatch();
        try
        {
            init?.Invoke(instance, null);
            
            sw.Start();

            Action executeTest = () => 
            {
                var result = method.Invoke(instance, parameters);
                if (result is Task task) task.GetAwaiter().GetResult();
            };

            if (timeoutMs > 0)
            {
                var task = Task.Run(executeTest);
                if (!task.Wait(timeoutMs))
                {
                    throw new TimeoutException($"Test execution exceeded the timeout of {timeoutMs} ms.");
                }
            }
            else
            {
                executeTest();
            }
            
            sw.Stop();
            TestReporter.PrintSuccess(testDisplayName, sw.ElapsedMilliseconds);
            passed++;
        }
        catch (Exception e)
        {
            sw.Stop();
            if (e is AggregateException aggEx) e = aggEx.InnerException ?? e;
            if (e is TargetInvocationException tie) e = tie.InnerException ?? e;

            if (e is AssertFailedException assertEx)
            {
                TestReporter.PrintFailure(testDisplayName, assertEx.Message);
            }
            else if (e is TimeoutException timeoutEx)
            {
                TestReporter.PrintFailure(testDisplayName, timeoutEx.Message);
            }
            else 
            {
                TestReporter.PrintFailure(testDisplayName, $"Unhandled Exception: {e.GetType().Name} - {e.Message}");
            }
            failed++;
        }
        finally
        {
            try { cleanup?.Invoke(instance, null); }
            catch (Exception cleanupEx)
            {
                var realError = cleanupEx is TargetInvocationException tie ? tie.InnerException : cleanupEx;
                TestReporter.PrintError($"Cleanup failed for {testDisplayName}: {realError?.Message}");
            }
        }
    }
}
