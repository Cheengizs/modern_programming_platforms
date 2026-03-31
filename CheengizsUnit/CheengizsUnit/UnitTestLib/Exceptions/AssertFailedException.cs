namespace UnitTestLib.Exceptions;

public class AssertFailedException : Exception
{
    public AssertFailedException(string msg) :  base(msg)
    {
        
    }
}
