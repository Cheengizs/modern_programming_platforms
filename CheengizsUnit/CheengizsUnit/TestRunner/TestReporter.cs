namespace TestRunner;

public static class TestReporter
{
    private static readonly object _consoleLock = new object();

    public static void PrintHeader(string message)
    {
        lock (_consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n▶ {message}");
            Console.ResetColor();
        }
    }

    public static void PrintSuccess(string testName, long elapsedMs)
    {
        lock (_consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  [PASS] {testName} ({elapsedMs} ms) [Thread: {Environment.CurrentManagedThreadId}]");
            Console.ResetColor();
        }
    }

    public static void PrintFailure(string testName, string message)
    {
        lock (_consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [FAIL] {testName}");
            Console.WriteLine($"         -> {message}");
            Console.ResetColor();
        }
    }

    public static void PrintError(string message)
    {
        lock (_consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[ERROR] {message}");
            Console.ResetColor();
        }
    }

    public static void PrintSummary(int passed, int failed, long totalElapsedMs)
    {
        lock (_consoleLock)
        {
            Console.WriteLine("\n======================================");
            Console.ForegroundColor = failed > 0 ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine($" Total: {passed + failed} | Passed: {passed} | Failed: {failed}");
            Console.WriteLine($" Execution Time: {totalElapsedMs} ms");
            Console.ResetColor();
            Console.WriteLine("======================================");
        }
    }
}
