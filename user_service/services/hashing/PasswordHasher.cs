using System.Security.Cryptography;

namespace user_service.services.hashing;

public class PasswordHasher
{
    private const int SaltSize = 64; //128 - 8, длина в байтах
    private const int KeySize = 128; //256 - 8, длина в байтах
    private const int Iterations = 10000; // Кол-во итераций
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
    private const char SaltDelimeter = ';';

    public static string GenerateHash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);

        return string.Join(SaltDelimeter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public static bool HashIsPassword(string passwordHash, string password)
    {
        var pwdElements = passwordHash.Split(SaltDelimeter);
        var salt = Convert.FromBase64String(pwdElements[0]);
        var hash = Convert.FromBase64String(pwdElements[1]);

        var hashInput = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);

        return CryptographicOperations.FixedTimeEquals(hash, hashInput);
    }
}
