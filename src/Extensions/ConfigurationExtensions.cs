namespace ForwardAuthGateway.Extensions;

public static class ConfigurationExtensions
{
    public static T AddConfigurationFromSectionName<T>(this IServiceCollection services, IConfiguration configuration, string sectionName)
        where T : class
    {
        var configOptions = configuration.GetSection(sectionName);

        services
            .AddOptions<T>()
            .Bind(configOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return configOptions.Get<T>()!;
    }
}
