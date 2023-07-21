using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ForwardAuthServer.Api.Authorization;

public static class CurrentUserExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddScoped<CurrentUser>();
        services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
        services.AddScoped<IClaimMappingService, ClaimMappingService>();
        return services;
    }

    private sealed class ClaimsTransformation : IClaimsTransformation
    {
        private readonly CurrentUser _currentUser;

        public ClaimsTransformation(CurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var type= principal.Identity?.AuthenticationType;

            _currentUser.Principal = principal;

            return Task.FromResult(principal);
        }
    }
}