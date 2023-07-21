using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace ForwardAuthGateway.Options;

public class ClaimTransformationOptions
{
    public ICollection<ClaimTransformation>? Transformations { get; set; } = default!;
}

public class ClaimTransformation
{
    public TransformationType TransformationType { get; set; }
    public string OriginClaimType { get; set; } = string.Empty;
    public string TargetClaimType { get; set; } = string.Empty;

    public void AddClaimActions(ClaimActionCollection collection)
    {
        switch (TransformationType)
        {
            case TransformationType.Map:
                collection.MapUniqueJsonKey(TargetClaimType, OriginClaimType);
                break;
            case TransformationType.MapAndRemoveOrigin:
                collection.MapUniqueJsonKey(TargetClaimType, OriginClaimType);
                collection.Remove(OriginClaimType);
                break;
            case TransformationType.Remove:
                collection.Remove(OriginClaimType);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

public enum TransformationType
{
    Map,
    MapAndRemoveOrigin,
    Remove,
}