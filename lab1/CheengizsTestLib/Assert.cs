using CheengizsTestLib.Exceptions;

namespace CheengizsTestLib;

public class Assert
{
    public static void IsEqual<T>(T expected, T actual, string message = "")
    {
        if (!Equals(expected, actual))
        {
            throw new AssertFailedException(
                $"Expected: <{expected}>. Actual: <{actual}>. {message}");
        }
    }

    public static void IsNotEqual<T>(T notExpected, T actual, string message = "")
    {
        if (Equals(notExpected, actual))
        {
            throw new AssertFailedException(
                $"Expected any value except: <{notExpected}>. Actual: <{actual}>. {message}");
        }
    }

    public static void IsTrue(bool condition, string message = "")
    {
        if (!condition)
        {
            throw new AssertFailedException($"Expected: True. Actual: False. {message}");
        }
    }

    public static void IsFalse(bool condition, string message = "")
    {
        if (condition)
        {
            throw new AssertFailedException($"Expected: False. Actual: True. {message}");
        }
    }

    public static void IsNull(object obj, string message = "")
    {
        if (obj != null)
        {
            throw new AssertFailedException($"Expected: Null. Actual: Not Null. {message}");
        }
    }

    public static void IsNotNull(object obj, string message = "")
    {
        if (obj == null)
        {
            throw new AssertFailedException($"Expected: Not Null. Actual: Null. {message}");
        }
    }

    public static void IsInstanceOf<T>(object obj, string message = "")
    {
        if (obj is not T)
        {
            throw new AssertFailedException(
                $"Expected type: <{typeof(T).Name}>. Actual type: <{obj?.GetType().Name ?? "null"}>. {message}");
        }
    }

    public static void IsEmpty<T>(IEnumerable<T> collection, string message = "")
    {
        using var enumerator = collection.GetEnumerator();
        if (enumerator.MoveNext())
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
                $"Expected exception: <{typeof(TException
                ).Name}>. Actual: <{ex.GetType().Name}>. {message}");
        }

        throw new AssertFailedException(
            $"Expected exception: <{typeof(TException).Name}> but no exception was thrown. {message}");
    }

    public static async Task<T> ThrowsAsync<T>(Func<Task> action, string message = "") where T : Exception
    {
        try
        {
            await action();
        }
        catch (T ex)
        {
            return ex;
        }
        catch (Exception ex)
        {
            throw new AssertFailedException(
                $"Expected exception: <{typeof(T).Name}>. Actual: <{ex.GetType().Name}>. {message}");
        }

        throw new AssertFailedException(
            $"Expected exception: <{typeof(T).Name}> but no exception was thrown. {message}");
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
            throw new AssertFailedException($"Collection does not contain expected item: <{expectedItem}>. {message}");
        }
    }
    
    public static void Single<T>(ICollection<T> collection, string message = "")
    {
        if (collection.Count != 1)
        {
            throw new AssertFailedException("1", collection.Count.ToString(),
                $"Expected collection to contain exactly one item. {message}");
        }
    }

    public static void NotSingle<T>(ICollection<T> collection, string message = "")
    {
        if (collection.Count == 1)
        {
            throw new AssertFailedException("!=1", "1", $"Expected collection to contain exactly one item. {message}");
        }
    }

    public static void SpecificThrow(string message)
    {
        throw new AssertFailedException(message);
    }
}