using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace HMSMini.API.Swagger;

/// <summary>
/// Operation filter to handle IFormFile parameters in Swagger
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if this operation has file upload parameters
        var hasFileParameter = context.ApiDescription.ParameterDescriptions
            .Any(p => p.Type == typeof(IFormFile));

        if (!hasFileParameter)
            return;

        // Get all parameters from the action
        var parameters = new Dictionary<string, OpenApiSchema>();

        foreach (var param in context.ApiDescription.ParameterDescriptions)
        {
            if (param.Type == typeof(IFormFile))
            {
                // File parameter
                parameters[param.Name] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                };
            }
            else if (param.Type == typeof(int))
            {
                // Integer parameter
                parameters[param.Name] = new OpenApiSchema
                {
                    Type = "integer",
                    Format = "int32"
                };
            }
            else
            {
                // String or other types default to string
                parameters[param.Name] = new OpenApiSchema
                {
                    Type = "string"
                };
            }
        }

        // Clear parameters and set request body
        operation.Parameters.Clear();
        operation.RequestBody = new OpenApiRequestBody
        {
            Required = true,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = parameters
                    }
                }
            }
        };
    }
}
