using UnitTestLib.Exceptions;
using System.Linq.Expressions;

namespace UnitTestLib.Asserts;

public class AssertResult
{
    
    public static void IsEqual<T>(T expected, T actual, string message = "")
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new AssertFailedException($"Expected: <{expected}>. Actual: <{actual}>. {message}");
        }
    }

    public static void IsNotEqual<T>(T notExpected, T actual, string message = "")
    {
        if (EqualityComparer<T>.Default.Equals(notExpected, actual))
        {
            throw new AssertFailedException($"Expected any value except: <{notExpected}>. Actual: <{actual}>. {message}");
        }
    }

    public static void IsTrue(bool condition, string message = "")
    {
        if (!condition) throw new AssertFailedException($"Expected: True. Actual: False. {message}");
    }

    public static void IsFalse(bool condition, string message = "")
    {
        if (condition) throw new AssertFailedException($"Expected: False. Actual: True. {message}");
    }

    public static void IsNull(object? obj, string message = "")
    {
        if (obj != null) throw new AssertFailedException($"Expected: Null. Actual: Not Null. {message}");
    }

    public static void IsNotNull(object? obj, string message = "")
    {
        if (obj == null) throw new AssertFailedException($"Expected: Not Null. Actual: Null. {message}");
    }

    public static void IsInstanceOf<T>(object? obj, string message = "")
    {
        if (obj is not T)
        {
            throw new AssertFailedException(
                $"Expected type: <{typeof(T).Name}>. Actual type: <{obj?.GetType().Name ?? "null"}>. {message}");
        }
    }

    public static void IsEmpty<T>(IEnumerable<T> collection, string message = "")
    {
        if (collection.Any())
        {
            throw new AssertFailedException($"Expected empty collection. Actual: contains items. {message}");
        }
    }

    public static TException Throws<TException>(Action action, string message = "") where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException ex)
        {
            return ex;
        }
        catch (Exception ex)
        {
            throw new AssertFailedException(
                $"Expected exception: <{typeof(TException).Name}>. Actual: <{ex.GetType().Name}>. {message}");
        }

        throw new AssertFailedException(
            $"Expected exception: <{typeof(TException).Name}> but no exception was thrown. {message}");
    }

    public static async Task<TException> ThrowsAsync<TException>(Func<Task> action, string message = "") where TException : Exception
    {
        try
        {
            await action();
        }
        catch (TException ex)
        {
            return ex;
        }
        catch (Exception ex)
        {
            throw new AssertFailedException(
                $"Expected exception: <{typeof(TException).Name}>. Actual: <{ex.GetType().Name}>. {message}");
        }

        throw new AssertFailedException(
            $"Expected exception: <{typeof(TException).Name}> but no exception was thrown. {message}");
    }

    public static void Contains<T>(IEnumerable<T> collection, T expectedItem, string message = "")
    {
        if (!collection.Contains(expectedItem))
        {
            throw new AssertFailedException($"Collection does not contain expected item: <{expectedItem}>. {message}");
        }
    }

    public static void NotContains<T>(IEnumerable<T> collection, T expectedItem, string message = "")
    {
        if (collection.Contains(expectedItem))
        {
            throw new AssertFailedException($"Collection contains unexpected item: <{expectedItem}>. {message}");
        }
    }

    public static void Single<T>(IEnumerable<T> collection, string message = "")
    {
        int count = collection.Count();
        if (count != 1)
        {
            throw new AssertFailedException($"Expected collection to contain exactly one item. Actual count: {count}. {message}");
        }
    }

    public static void NotSingle<T>(IEnumerable<T> collection, string message = "")
    {
        int count = collection.Count();
        if (count == 1)
        {
            throw new AssertFailedException($"Expected collection to NOT contain exactly one item. Actual count: 1. {message}");
        }
    }

    public static void Fail(string message) => throw new AssertFailedException(message);
    
    public static void Check(Expression<Func<bool>> condition, string message = "")
    {
        var func = condition.Compile();
        if (!func())
        {
            if (condition.Body is BinaryExpression binary)
            {
                var leftValue = Expression.Lambda(binary.Left).Compile().DynamicInvoke();
                var rightValue = Expression.Lambda(binary.Right).Compile().DynamicInvoke();
            
                throw new AssertFailedException(
                    $"Expression Failed: {condition.Body}. " +
                    $"\nLeft Operand: <{leftValue}> " +
                    $"\nOperator: {binary.NodeType} " +
                    $"\nRight Operand: <{rightValue}>. " +
                    $"\n{message}");
            }
        
            throw new AssertFailedException($"Expression Failed: {condition.Body}. {message}");
        }
    }
}
