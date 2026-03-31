namespace TestRunner;

public static class InputHandler
{
    public enum ExecutionMode
    {
        Sync,
        ParallelForEach,
        CustomThreadPool
    }
    
    public static ExecutionMode AskForExecutionMode()
    {
        Console.WriteLine("\nChoose execution mode:");
        Console.WriteLine("1. Synchronous (Stable)");
        Console.WriteLine("2. Parallel.ForEach (Fast)");
        Console.WriteLine("3. Custom Thread Pool (Demonstrate Scaling & Load)");
        
        var key = Console.ReadKey(true);
        return key.KeyChar switch
        {
            '2' => ExecutionMode.ParallelForEach,
            '3' => ExecutionMode.CustomThreadPool,
            _ => ExecutionMode.Sync
        };
    }
    
    public static string GetAssemblyPath(string[] args)
    {
        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        {
            return args[0].Trim('"');
        }

        string userInput;
        do
        {
            Console.Write("Input assembly path: ");
            userInput = Console.ReadLine() ?? string.Empty;
        } 
        while (string.IsNullOrWhiteSpace(userInput));

        return userInput.Trim('"');
    }
    
    public static bool AskForParallelMode()
    {
        Console.WriteLine("Choose execution mode:");
        Console.WriteLine("1. Synchronous (Stable)");
        Console.WriteLine("2. Parallel (Fast)");
        var key = Console.ReadKey(true);
        return key.KeyChar == '2';
    }
    
    public static int GetMaxDegreeOfParallelism()
    {
        Console.Write($"Input Max Degree of Parallelism (Default is {Environment.ProcessorCount}): ");
        string input = Console.ReadLine() ?? "";
        
        if (int.TryParse(input, out int result) && result > 0)
        {
            return result;
        }

        return Environment.ProcessorCount;
    }

}
