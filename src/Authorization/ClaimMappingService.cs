using ForwardAuthGateway.Exceptions;
using ForwardAuthGateway.Options;
using Microsoft.Extensions.Options;

namespace ForwardAuthGateway.Authorization;

interface IClaimMappingService
{
    IDictionary<string, string> MapCurrentUserClaimsToHeaders();
    IDictionary<string, string> MapCurrentUserClaimsToUserInfo();
}

public class ClaimMappingService : IClaimMappingService
{
    private readonly ILogger<ClaimMappingService> _logger;
    private readonly ForwardAuthOptions _forwardAuthOptions;
    private readonly CurrentUser _currentUser;

    public ClaimMappingService(ILogger<ClaimMappingService> logger,
        IOptions<ForwardAuthOptions> forwardAuthOptions,
        CurrentUser currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
        _forwardAuthOptions = forwardAuthOptions.Value;
    }

    public IDictionary<string, string> MapCurrentUserClaimsToHeaders()
    {
        return Map(_forwardAuthOptions.ClaimToHeaderMappings.Mappings,
            _forwardAuthOptions.ClaimToHeaderMappings.MissingClaimAction);
    }

    public IDictionary<string, string> MapCurrentUserClaimsToUserInfo()
    {
        return Map(_forwardAuthOptions.ClaimToUserInfoMappings.Mappings,
            _forwardAuthOptions.ClaimToUserInfoMappings.MissingClaimAction);
    }

    private IDictionary<string, string> Map(Dictionary<string, string> inputMappings,
        MissingClaimAction missingClaimAction)
    {
        var mappedHeaders = new Dictionary<string, string>();
        foreach (var (claimName, headerKey) in inputMappings)
        {
            if (_currentUser.Principal.Claims.FirstOrDefault(c => c.Type == claimName) is { } claim)
            {
                mappedHeaders[headerKey] = claim.Value;
            }
            else
            {
                switch (missingClaimAction)
                {
                    case MissingClaimAction.Ignore:
                        break;
                    case MissingClaimAction.Warn:
                        _logger.LogWarning(
                            "Expected to find claim with name '{claimName}' in the claims principal.", claimName);
                        break;
                    case MissingClaimAction.Throw:
                        throw new MissingClaimException(claimName);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        return mappedHeaders;
    }
}