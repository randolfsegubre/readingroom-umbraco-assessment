namespace ReadingRoom.ArticleCMS.Core.Constants;

public static class ApiKeyAuthentication
{
    /// <summary>
    /// The request header that carries the key. Umbraco's Delivery API reads the same header.
    /// </summary>
    public const string HeaderName = "Api-Key";

    /// <summary>
    /// The name of the security scheme in the Swagger documents.
    /// </summary>
    public const string SecuritySchemeName = "ApiKey";
}
