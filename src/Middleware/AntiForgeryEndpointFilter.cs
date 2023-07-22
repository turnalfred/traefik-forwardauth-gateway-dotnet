using ForwardAuthGateway.Exceptions;

namespace ForwardAuthGateway.Middleware;

class AntiForgeryEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // TODO only apply to our local endpoints and skip and that are simply being proxied downstream?
        // -> downstream services would need to check the header themselves
        // -> does this apply to application routes (main page loads) which aren't able to add the X-CSRF-Token header?
        
        // TODO validate anti-forgery token approach using static value. Wikipedia suggests this should always be unique
        // -> is it out of date? https://en.wikipedia.org/wiki/Cross-site_request_forgery#Synchronizer_token_pattern
        var header = context.HttpContext.Request.Headers.FirstOrDefault(x =>
            string.Equals(x.Key, "CSRF-Token", StringComparison.InvariantCultureIgnoreCase));
        
        if (string.IsNullOrEmpty(header.Value) || header.Value != 1.ToString())
        {
            throw new AntiForgeryTokenMissingException();
        }

        return await next(context);
    }
}