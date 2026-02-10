using System.Reflection;
using CheengizsTestLib.Attributes;
using CheengizsTestLib.Exceptions;

namespace TestRunner;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("start");

        string dllPath = args[0];
        if (!File.Exists(dllPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"error:file not found {dllPath}");
            return;
        }

        try
        {
            var assembly = Assembly.LoadFrom(dllPath);
            Console.WriteLine($"dll loaded: {assembly.FullName}");
            Console.WriteLine("_____________________________________");

            int totalTests = 0;
            int passedTests = 0;
            int failedTests = 0;

            var testClasses = assembly.GetTypes().Where(t => t.GetCustomAttribute(typeof(TestClassAttribute)) != null);

            foreach (var testType in testClasses)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"\ntest class: {testType.Name}");
                Console.ResetColor();


                var initMethod = testType.GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttribute(typeof(TestInitial)) != null);
                var endMethod = testType.GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttribute(typeof(TestEnd)) != null);

                var testMethods = testType.GetMethods()
                    .Where(m => m.GetCustomAttribute(typeof(TestMethodAttribute)) != null);

                foreach (var method in testMethods)
                {
                    var dataAttributes = method.GetCustomAttributes<DataAttribute>().ToList();
                    IEnumerable<object[]> scenarios;
                    if (dataAttributes.Any())
                    {
                        scenarios = dataAttributes.Select(attr => attr.Data);
                    }
                    else
                    {
                        scenarios = new List<object[]> { null };
                    }

                    foreach (var parameters in scenarios)
                    {
                        totalTests++;
                        string paramsInfo = parameters != null ? $"({string.Join(", ", parameters)})" : "";
                        Console.Write($"test method: {method.Name}{paramsInfo} ");

                        var testInstance = Activator.CreateInstance(testType);

                        try
                        {
                            initMethod?.Invoke(testInstance, null);

                            var res = method.Invoke(testInstance, parameters);

                            if (res is Task task)
                            {
                                task.Wait();
                            }

                            PrintSuccess("result: success");
                            passedTests++;
                        }
                        catch (TargetInvocationException tie)
                        {
                            PrintFail("result: fail");

                            var message = tie.InnerException is AssertFailedException
                                ? tie.InnerException.Message
                                : tie.InnerException?.Message ?? tie.Message;

                            Console.WriteLine($"reason: {message}");
                            failedTests++;
                        }
                        catch (Exception ex)
                        {
                            PrintFail("result: error");
                            Console.WriteLine("unexpected error: " + ex.Message);
                            failedTests++;
                        }
                        finally
                        {
                            try
                            {
                                endMethod?.Invoke(testInstance, null);
                            }
                            catch
                            {
                                
                            }
                        }
                    }
                }
            }

            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine($"Total: {totalTests}, Success: {passedTests}, Failed: {failedTests}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("unknown errro: " + ex.Message);
        }
    }


    static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    static void PrintFail(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}