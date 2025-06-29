namespace Forum.Statics;

using System.Security.Cryptography;

public static class Constants
{
    public const int PasswordByteLength = SHA3_512.HashSizeInBytes;

    public const string AuthCookie = "auth";
    public const string AlertMessage = "alertmessage";
}