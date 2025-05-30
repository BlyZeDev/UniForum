namespace Forum.Statics;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

public static class Util
{
    public static bool Equals(Password left, Password right)
    {
        ReadOnlySpan<byte> leftSpan = left;
        ReadOnlySpan<byte> rightSpan = right;

        return leftSpan.SequenceEqual(rightSpan);
    }

    public static Password HashPassword(string password)
    {
        Span<byte> passwordBytes = stackalloc byte[Encoding.UTF8.GetMaxByteCount(password.Length)];
        Encoding.UTF8.GetBytes(password, passwordBytes);

        Span<byte> hashed = stackalloc byte[SHA3_256.HashSizeInBytes];
        SHA3_512.HashData(passwordBytes, hashed);

        return Unsafe.ReadUnaligned<Password>(ref MemoryMarshal.GetReference(hashed));
    }
}