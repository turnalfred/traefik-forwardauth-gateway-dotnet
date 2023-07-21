using ForwardAuthServer.Api.Authentication;
using ForwardAuthServer.Api.Authorization;
using ForwardAuthServer.Api.Endpoints;
using ForwardAuthServer.Api.Middleware;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddForwardAuth(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddCurrentUser();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedHost
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapLoginEndpoints();
app.MapUserEndpoints();
app.MapAuthCheckEndpoints();

app.Run();