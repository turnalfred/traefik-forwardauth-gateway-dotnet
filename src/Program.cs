using ForwardAuthGateway.Authentication;
using ForwardAuthGateway.Authorization;
using ForwardAuthGateway.Endpoints;
using ForwardAuthGateway.Middleware;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddForwardAuth(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddCurrentUser();
builder.Services.AddScoped<AntiForgeryEndpointFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedHost
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapAuthCheckEndpoints();
app.MapLoginEndpoints();

// applying EndpointFilters only to endpoints being consumed in a headless fashion. see TODO in AntiForgeryEndpointFilter.cs
app.MapProviderEndpoints()
    .AddEndpointFilter<AntiForgeryEndpointFilter>();

app.MapUserEndpoints()
    .AddEndpointFilter<AntiForgeryEndpointFilter>();

app.Run();