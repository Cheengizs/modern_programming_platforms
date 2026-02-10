namespace CheengizsTestLib.Exceptions;

public class AssertFailedException : Exception
{
    public AssertFailedException(string message) : base(message)
    {
    }

    public AssertFailedException(string expected, string actual, string message = "")
        : base($"Assert failed. {message} Expected: <{expected}>. Actual: <{actual}>.")
    {
    }
}