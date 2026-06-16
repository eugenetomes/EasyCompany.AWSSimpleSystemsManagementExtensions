using Microsoft.Extensions.Configuration;

namespace EasyCompany.Extensions.Configuration.AWSSimpleSystemsManagement.Internal.SimpleSystemsManagementJson;

internal class SimpleSystemsManagementJsonConfigurationSource : IConfigurationSource
{
    private readonly string _region;
    private readonly string _parameterName;
    private readonly string _jsonObjectName;
    private readonly bool _isEncrypted;
    private readonly bool _optional;
    private readonly string _accessKeyId = string.Empty;
    private readonly string _secretAccessKey = string.Empty;

    public SimpleSystemsManagementJsonConfigurationSource(string region, string parameterName, string jsonObjectName, bool isEncrypted, bool optional)
    {
        _region = region;
        _parameterName = parameterName;
        _jsonObjectName = jsonObjectName;
        _isEncrypted = isEncrypted;
        _optional = optional;
    }

    public SimpleSystemsManagementJsonConfigurationSource(string accessKeyId, string secretAccessKey, string region, string parameterName, string jsonObjectName, bool isEncrypted, bool optional)
    {
        _accessKeyId = accessKeyId;
        _secretAccessKey = secretAccessKey;
        _region = region;
        _parameterName = parameterName;
        _jsonObjectName = jsonObjectName;
        _isEncrypted = isEncrypted;
        _optional = optional;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(_accessKeyId))
        {
            return new SimpleSystemsManagementJsonConfigurationProvider(_region, _parameterName, _jsonObjectName, _isEncrypted, _optional);
        }
        return new SimpleSystemsManagementJsonConfigurationProvider(_accessKeyId, _secretAccessKey, _region, _parameterName, _jsonObjectName, _isEncrypted, _optional);
    }
}
