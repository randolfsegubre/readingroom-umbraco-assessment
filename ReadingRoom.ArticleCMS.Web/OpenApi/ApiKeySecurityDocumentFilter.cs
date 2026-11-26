using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ReadingRoom.ArticleCMS.Web.OpenApi;

/// <summary>
/// Adds the API key header to the Swagger documents that need it, so the Authorize button
/// in the Swagger page can send the key.
/// </summary>
public sealed class ApiKeySecurityDocumentFilter : IDocumentFilter
{
    private readonly IReadOnlyCollection<string> _documentNames;

    public ApiKeySecurityDocumentFilter(IReadOnlyCollection<string> documentNames)
    {
        _documentNames = documentNames;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (!_documentNames.Contains(context.DocumentName))
        {
            return;
        }

        swaggerDoc.Components ??= new OpenApiComponents();
        swaggerDoc.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        swaggerDoc.Components.SecuritySchemes[ApiKeyAuthentication.SecuritySchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = ApiKeyAuthentication.HeaderName,
            Description = "The API key from the configuration section Umbraco:CMS:DeliveryApi."
        };

        foreach (IOpenApiPathItem pathItem in swaggerDoc.Paths.Values)
        {
            foreach (OpenApiOperation operation in pathItem.Operations?.Values ?? Enumerable.Empty<OpenApiOperation>())
            {
                operation.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(ApiKeyAuthentication.SecuritySchemeName, swaggerDoc)] = new List<string>()
                });
            }
        }
    }
}
