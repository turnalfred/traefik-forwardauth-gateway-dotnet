namespace ForwardAuthGateway.Exceptions;

public class AntiForgeryTokenMissingException : UnauthorizedException
{
    public AntiForgeryTokenMissingException() : base("Missing anti-forgery token")
    {
    }
}