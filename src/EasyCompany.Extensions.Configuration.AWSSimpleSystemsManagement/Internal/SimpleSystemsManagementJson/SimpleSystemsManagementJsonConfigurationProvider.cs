using Amazon;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace EasyCompany.Extensions.Configuration.AWSSimpleSystemsManagement.Internal.SimpleSystemsManagementJson;

internal class SimpleSystemsManagementJsonConfigurationProvider : ConfigurationProvider
{
    private readonly string _region;
    private readonly string _parameterName;
    private readonly string _jsonObjectName;
    private readonly bool _isEncrypted;
    private readonly bool _optional;
    private readonly string _accessKeyId = string.Empty;
    private readonly string _secretAccessKey = string.Empty;


    public SimpleSystemsManagementJsonConfigurationProvider(string region, string parameterName, string jsonObjectName, bool isEncrypted, bool optional)
    {
        _region = region;
        _parameterName = parameterName;
        _jsonObjectName = jsonObjectName;
        _isEncrypted = isEncrypted;
        _optional = optional;
    }

    public SimpleSystemsManagementJsonConfigurationProvider(string accessKeyId, string secretAccessKey, string region, string parameterName, string jsonObjectName, bool isEncrypted, bool optional)
    {
        _accessKeyId = accessKeyId;
        _secretAccessKey = secretAccessKey;
        _region = region;
        _parameterName = parameterName;
        _jsonObjectName = jsonObjectName;
        _isEncrypted = isEncrypted;
        _optional = optional;
    }

    public override void Load()
    {
        try
        {
            var secret = GetParameterValue();
            if (string.IsNullOrEmpty(secret))
            {
                throw new InvalidOperationException($"Parameter '{_parameterName}' not found or is empty.");
            }

            var jsonData = JObject.Parse(secret);
            var prefix = string.Empty;
            if (!string.IsNullOrEmpty(_jsonObjectName))
            {
                prefix = _jsonObjectName + ":";
            }

            if (jsonData is null)
            {
                return;
            }

            FlattenJson(jsonData, prefix);
        }
        catch (Exception)
        {
            if (_optional == false)
            {
                throw;
            }

        }
    }

    private void FlattenJson(JToken token, string prefix)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (var property in ((JObject)token).Properties())
                {
                    var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}:{property.Name}";
                    FlattenJson(property.Value, key);
                }
                break;
            case JTokenType.Array:
                var array = (JArray)token;
                for (int i = 0; i < array.Count; i++)
                {
                    FlattenJson(array[i], $"{prefix}:{i}");
                }
                break;
            default:
                Data[prefix] = token.ToString();
                break;
        }
    }

    private string GetParameterValue()
    {
        var request = new GetParameterRequest
        {
            Name = _parameterName,
            WithDecryption = _isEncrypted  // Required for SecureString            
        };


        if (string.IsNullOrEmpty(_secretAccessKey))
        {
            using (var client = new AmazonSimpleSystemsManagementClient(RegionEndpoint.GetBySystemName(_region)))
            {
                var response = client.GetParameterAsync(request).Result;

                return response.Parameter.Value;
            }
        }

        var credentials = new BasicAWSCredentials(_accessKeyId, _secretAccessKey);
        using (var client = new AmazonSimpleSystemsManagementClient(credentials, RegionEndpoint.GetBySystemName(_region)))
        {
            var response = client.GetParameterAsync(request).Result;

            return response.Parameter.Value;
        }

    }
}
