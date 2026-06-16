# EasyCompany.AWSSimpleSystemsManagementExtensions

A .NET configuration provider extension for AWS Systems Manager Parameter Store that seamlessly integrates with `Microsoft.Extensions.Configuration`.

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)

## Overview

This library provides an easy way to load configuration values from AWS Systems Manager Parameter Store into your .NET applications. It supports JSON parameters with nested structures and automatically flattens them into the standard .NET configuration format.

## Features

- ✅ Load JSON parameters from AWS Parameter Store
- ✅ Support for nested JSON structures (objects and arrays)
- ✅ Automatic flattening to .NET configuration key format
- ✅ Support for encrypted parameters (SecureString)
- ✅ Optional parameter loading (won't fail if parameter doesn't exist)
- ✅ Multiple authentication methods (IAM roles, access keys)
- ✅ Seamless integration with `Microsoft.Extensions.Configuration`

## Installation

```bash
dotnet add package EasyCompany.Extensions.Configuration.AWSSimpleSystemsManagement
```

## Usage

### Basic Usage with IAM Authentication

```csharp
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddSimpleSystemsManagementJson(
        region: "us-east-1",
        parameterName: "/myapp/config",
        jsonObjectName: "MySettings",
        isEncrypted: false,
        optional: true)
    .Build();

// Access configuration values
var connectionString = configuration["MySettings:ConnectionStrings:DefaultConnection"];
var apiKey = configuration["MySettings:ApiKey"];
```

### Usage with Access Keys

```csharp
var configuration = new ConfigurationBuilder()
    .AddSimpleSystemsManagementJson(
        accessKeyId: "YOUR_ACCESS_KEY",
        secretAccessKey: "YOUR_SECRET_KEY",
        region: "us-east-1",
        parameterName: "/myapp/config",
        jsonObjectName: "MySettings",
        isEncrypted: false,
        optional: true)
    .Build();
```

### Parameter Store JSON Format

Store your configuration in AWS Parameter Store as JSON. The provider automatically flattens nested structures:

#### Parameter Store Value:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mydb;",
    "RedisConnection": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "ApiKeys": ["key1", "key2", "key3"]
}
```

#### Resulting Configuration Keys:
```
MySettings:ConnectionStrings:DefaultConnection = "Server=localhost;Database=mydb;"
MySettings:ConnectionStrings:RedisConnection = "localhost:6379"
MySettings:Logging:LogLevel:Default = "Information"
MySettings:Logging:LogLevel:Microsoft = "Warning"
MySettings:ApiKeys:0 = "key1"
MySettings:ApiKeys:1 = "key2"
MySettings:ApiKeys:2 = "key3"
```

## Configuration Options

| Parameter | Type | Description |
|-----------|------|-------------|
| `region` | `string` | AWS region where the parameter is stored (e.g., "us-east-1") |
| `parameterName` | `string` | The name/path of the parameter in Parameter Store (e.g., "/myapp/config") |
| `jsonObjectName` | `string` | Optional prefix for all configuration keys |
| `isEncrypted` | `bool` | Whether the parameter is encrypted (SecureString). Default: `false` |
| `optional` | `bool` | If `true`, won't throw an exception if the parameter doesn't exist. Default: `false` |

## Encrypted Parameters

To work with encrypted parameters (SecureString type in Parameter Store):

```csharp
.AddSimpleSystemsManagementJson(
    region: "us-east-1",
    parameterName: "/myapp/secrets",
    jsonObjectName: "Secrets",
    isEncrypted: true,  // Enable decryption
    optional: false)
```

**Note:** Make sure your IAM role or user has the `kms:Decrypt` permission for the KMS key used to encrypt the parameter.

## AWS Permissions Required

Your application needs the following IAM permissions:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "ssm:GetParameter"
      ],
      "Resource": "arn:aws:ssm:REGION:ACCOUNT_ID:parameter/YOUR_PARAMETER_PATH"
    }
  ]
}
```

For encrypted parameters, also add:

```json
{
  "Effect": "Allow",
  "Action": [
    "kms:Decrypt"
  ],
  "Resource": "arn:aws:kms:REGION:ACCOUNT_ID:key/YOUR_KMS_KEY_ID"
}
```

## ASP.NET Core Integration

In your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSimpleSystemsManagementJson(
    region: builder.Configuration["AWS:Region"] ?? "us-east-1",
    parameterName: builder.Configuration["AWS:ParameterName"] ?? "/myapp/config",
    jsonObjectName: "AppSettings",
    isEncrypted: true,
    optional: false);

var app = builder.Build();
```

## Error Handling

The provider throws an `InvalidOperationException` if:
- The parameter doesn't exist and `optional` is set to `false`
- The parameter value is empty or null
- The JSON format is invalid

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Authors

- Eugene Tomes - [TomesDev](https://github.com/eugenetomes)

## Repository

[https://github.com/eugenetomes/EasyCompany.AWSSimpleSystemsManagementExtensions](https://github.com/eugenetomes/EasyCompany.AWSSimpleSystemsManagementExtensions)
