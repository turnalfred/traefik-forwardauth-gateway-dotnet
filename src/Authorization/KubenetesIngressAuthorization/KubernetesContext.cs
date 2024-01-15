using System.Security.Claims;
using k8s;

public class KubernetesContext
{
    private readonly Kubernetes kubernetes;
    public KubernetesContext(IWebHostEnvironment webHostEnvironment){
        KubernetesClientConfiguration kubeConfig;
        if(webHostEnvironment.IsDevelopment()){
            kubeConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile();
        }else{
            kubeConfig = KubernetesClientConfiguration.InClusterConfig();
        }
        kubernetes = new Kubernetes(kubeConfig);
    }

    internal bool IsIngressGroup(ClaimsPrincipal user)
    {
        throw new NotImplementedException();
    }

    void GetIngresses(){
        var ingresses = kubernetes.ListIngressForAllNamespaces();

    }

}