using Azure.Identity;

namespace BluePrintApi;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder LoadConfigurations(this IConfigurationBuilder builder)
    {
        builder.SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", false)
            .EnableKeyVault();
        return builder;
    }

    public static void EnableKeyVault(this IConfigurationBuilder config)
    {
        // Load environment variables
        config.AddEnvironmentVariables();

        IConfigurationRoot builtConfig = config.Build();
        string? keyVaultUrlBase = builtConfig[$"App:KeyVaultUrl"];

        if (string.IsNullOrEmpty(keyVaultUrlBase))
        {
            // If you do not want to use the values in KeyVault, make the kv url empty and put the values specifically in appsettings.json
            // But do not commit these appsettings.json changes.
            return;
        }

        // Use default azure credential to authenticate to key vault,
        // we need to turn off some of the default credential providers as they are not
        // available during local development.
        // NOTE: This requires the developer to perform `az login` in the terminal first or login to Visual Studio.
        // https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/credential-chains?tabs=dac#defaultazurecredential-overview
        bool isLocalDevelopment = builtConfig["ASPNETCORE_ENVIRONMENT"] == "Development";
        DefaultAzureCredentialOptions credOptions = new()
        {
            ExcludeEnvironmentCredential = isLocalDevelopment,
            ExcludeWorkloadIdentityCredential = isLocalDevelopment,
            ExcludeManagedIdentityCredential = isLocalDevelopment,
            ExcludeVisualStudioCredential = !isLocalDevelopment,
            ExcludeVisualStudioCodeCredential = !isLocalDevelopment
        };
        _ = config.AddAzureKeyVault(new Uri(keyVaultUrlBase), new DefaultAzureCredential(credOptions));
    }
}
