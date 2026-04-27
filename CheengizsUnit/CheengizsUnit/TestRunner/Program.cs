using System.Reflection;
using UnitTestLib.Attributes;

namespace TestRunner;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Cheengizs Test Runner ===");
        
        string path = InputHandler.GetAssemblyPath(args);
        InputHandler.ExecutionMode mode = InputHandler.AskForExecutionMode();
        
        int maxDegree = 1;
        if (mode != InputHandler.ExecutionMode.Sync)
        {
            maxDegree = InputHandler.GetMaxDegreeOfParallelism();
        }

        var executor = new TestExecutor();

        Func<MethodInfo, bool> filterDelegate = method => 
        {
            var categoryAttr = method.GetCustomAttribute<CategoryAttribute>();
            return categoryAttr == null || categoryAttr.Name != "Disabled"; 
        };

        executor.RunTests(path, mode, maxDegree, filterDelegate);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
