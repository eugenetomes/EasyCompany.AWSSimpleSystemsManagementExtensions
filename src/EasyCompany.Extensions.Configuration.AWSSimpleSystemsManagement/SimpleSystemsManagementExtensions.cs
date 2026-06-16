using EasyCompany.Extensions.Configuration.AWSSimpleSystemsManagement.Internal.SimpleSystemsManagementJson;

namespace Microsoft.Extensions.Configuration;

public static class SimpleSystemsManagementExtensions
{
    public static IConfigurationBuilder AddSimpleSystemsManagementJson(this IConfigurationBuilder builder, string region, string parameterName, string jsonObjectName, bool isEncrypted = false, bool optional = false)
    {
        return builder.Add(new SimpleSystemsManagementJsonConfigurationSource(region, parameterName, jsonObjectName, isEncrypted, optional));
    }

    public static IConfigurationBuilder AddSimpleSystemsManagementJson(this IConfigurationBuilder builder, string accessKeyId, string secretAccessKey, string region, string parameterName, string jsonObjectName, bool isEncrypted = false, bool optional = false)
    {
        return builder.Add(new SimpleSystemsManagementJsonConfigurationSource(accessKeyId, secretAccessKey, region, parameterName, jsonObjectName, isEncrypted, optional));
    }
}
