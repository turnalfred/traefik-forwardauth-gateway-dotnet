namespace ForwardAuthGateway.Exceptions;

public class MissingClaimException : KeyNotFoundException
{
    public MissingClaimException(string claimName) : base(
        $"Claim with name \"{claimName}\" was not found in the claims principal.")
    {

    }
}