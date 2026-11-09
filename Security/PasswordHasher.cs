using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

namespace AltunPital.Security;

public static class PasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public static string HashPassword(string password)
    {
        byte[] salt =
            RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return
            $"PBKDF2${Iterations}$" +
            $"{Convert.ToBase64String(salt)}$" +
            $"{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(
        string password,
        string storedHash)
    {
        if (!IsHashed(storedHash))
            return false;

        string[] parts =
            storedHash.Split('$');

        int iterations =
            int.Parse(parts[1]);

        byte[] salt =
            Convert.FromBase64String(parts[2]);

        byte[] expectedHash =
            Convert.FromBase64String(parts[3]);

        byte[] actualHash =
            Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

        return CryptographicOperations
            .FixedTimeEquals(
                actualHash,
                expectedHash);
    }

    public static bool IsHashed(
        string value)
    {
        return value.StartsWith(
            "PBKDF2$",
            StringComparison.Ordinal);
    }
}