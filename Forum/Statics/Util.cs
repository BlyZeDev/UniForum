namespace Forum.Statics;

using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

public static class Util
{
    public static string HashPassword(string password)
    {
        Span<byte> passwordBytes = stackalloc byte[Encoding.UTF8.GetMaxByteCount(password.Length)];
        Encoding.UTF8.GetBytes(password, passwordBytes);

        Span<byte> hashed = stackalloc byte[Base64.GetMaxEncodedToUtf8Length(SHA3_512.HashSizeInBytes)];
        SHA3_512.HashData(passwordBytes, hashed);

        Base64.EncodeToUtf8InPlace(hashed, SHA3_512.HashSizeInBytes, out var written);
        return Encoding.UTF8.GetString(hashed[..written]);
    }
}