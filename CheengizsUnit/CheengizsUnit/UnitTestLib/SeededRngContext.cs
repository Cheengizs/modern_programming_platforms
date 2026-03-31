namespace UnitTestLib;

public class SeededRngContext
{
    public Random Rng { get; }
    
    public SeededRngContext()
    {
        // Используем фиксированный сид (например, 42), 
        // чтобы последовательность "случайных" чисел всегда была одинаковой
        Rng = new Random(42); 
    }
}
