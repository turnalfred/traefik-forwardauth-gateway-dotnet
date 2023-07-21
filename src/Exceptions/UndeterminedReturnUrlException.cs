namespace ForwardAuthGateway.Exceptions;

public class UndeterminedReturnUrlException : InvalidOperationException
{
    public UndeterminedReturnUrlException(string attemptedUri)
        : base($"Unable to determine a well formed URL to return to after login. Received: \"{attemptedUri}\"")
    {

    }
}