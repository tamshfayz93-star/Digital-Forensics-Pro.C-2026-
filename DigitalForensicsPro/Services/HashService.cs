using System.Security.Cryptography;

namespace DigitalForensicsPro.Services;

public static class HashService
{
    public static string ComputeSha256(string path)
    {
        using var stream = File.OpenRead(path);
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(stream));
    }
}
