using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace ForwardAuthServer.Api.Authentication;

public class AddStaticClaimAction : ClaimAction
{
    private readonly string _staticValue = string.Empty;
    public AddStaticClaimAction(string claimType, string valueType, string staticValue) : base(claimType, valueType)
    {
        _staticValue = staticValue;
    }
    
    private AddStaticClaimAction(string claimType, string valueType) : base(claimType, valueType)
    {
    }

    public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
    {
        identity.AddClaim(new Claim(ClaimType, _staticValue, ValueType, issuer));
    }
}