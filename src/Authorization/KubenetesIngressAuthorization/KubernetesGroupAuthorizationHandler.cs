using Microsoft.AspNetCore.Authorization;

public class KubernetesGroupAuthorizationHandler : AuthorizationHandler<KubernetesGroupAuthorizationRequirement>
{
    private readonly KubernetesContext _kubeContext;

    public KubernetesGroupAuthorizationHandler(KubernetesContext kubeContext)
    {
        _kubeContext = kubeContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        KubernetesGroupAuthorizationRequirement requirement)
    {
        if (!_kubeContext.IsIngressGroup(context.User))
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
public class KubernetesGroupAuthorizationRequirement: IAuthorizationRequirement{}