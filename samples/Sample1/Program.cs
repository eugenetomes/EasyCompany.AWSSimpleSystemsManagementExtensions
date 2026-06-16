using Microsoft.Extensions.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        var parameterName = "/dev/test/connectionstring";
        var regionName = "eu-central-1";

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //.AddSecretsManagerConfiguration(regionName, secretName)
            .AddSimpleSystemsManagementJson(regionName, parameterName, "", isEncrypted: false, optional: true)
            .Build();
    }
}