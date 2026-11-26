using System.Security.Cryptography;
using System.Text;

namespace ReadingRoom.ArticleCMS.Core.Security;

public static class ApiKeyValidator
{
    /// <summary>
    /// Checks the key sent by a caller against the configured key.
    /// </summary>
    /// <param name="configuredKey">The key from the configuration. When it is empty, every request is refused.</param>
    /// <param name="providedKey">The key sent with the request.</param>
    /// <returns>True only when a key is configured and the provided key is the same.</returns>
    public static bool IsValid(string? configuredKey, string? providedKey)
    {
        if (string.IsNullOrWhiteSpace(configuredKey) || string.IsNullOrEmpty(providedKey))
        {
            return false;
        }

        // compare fixed length hashes in constant time, so the response time does not reveal the key.
        byte[] configuredHash = SHA256.HashData(Encoding.UTF8.GetBytes(configuredKey));
        byte[] providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(providedKey));

        return CryptographicOperations.FixedTimeEquals(configuredHash, providedHash);
    }
}
