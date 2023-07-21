using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedHost
});

app.MapGet("/api/hello",
    (ctx) =>
    {
        ctx.Response.WriteAsJsonAsync(new
        {
            Message = "Hello from the example service",
            RequestHeaders = ctx.Request.Headers.Where(x => x.Key.StartsWith("X-Auth")).OrderBy(x => x.Key)
        });
        
        return Task.CompletedTask;
    });

app.Run();