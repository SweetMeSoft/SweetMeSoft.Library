using System.Security.Cryptography;

namespace SweetMeSoft.Tools;

public class Security
{
    private const int IterationsV1 = 1000;
    private const int IterationsV2 = 600_000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public static string HashPasswordIrreversible(string password)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

#if NET6_0_OR_GREATER
        byte[] buffer2 = Rfc2898DeriveBytes.Pbkdf2(password, salt, IterationsV2, HashAlgorithmName.SHA256, KeySize);
#else
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, IterationsV2, HashAlgorithmName.SHA256);
        byte[] buffer2 = pbkdf2.GetBytes(KeySize);
#endif

        byte[] dst = new byte[1 + SaltSize + KeySize];
        dst[0] = 0x01; // Format marker v2
        Buffer.BlockCopy(salt, 0, dst, 1, SaltSize);
        Buffer.BlockCopy(buffer2, 0, dst, 1 + SaltSize, KeySize);
        return Convert.ToBase64String(dst);
    }

    public static bool VerifyHashedPasswordIrreversible(string base64HashedPassword, string cleanPassword)
    {
        if (base64HashedPassword == null)
        {
            return false;
        }
        if (cleanPassword == null)
        {
            throw new ArgumentNullException(nameof(cleanPassword));
        }

        byte[] src;
        try
        {
            src = Convert.FromBase64String(base64HashedPassword);
        }
        catch
        {
            return false;
        }

        if (src.Length != (1 + SaltSize + KeySize))
        {
            return false;
        }

        byte[] salt = new byte[SaltSize];
        Buffer.BlockCopy(src, 1, salt, 0, SaltSize);

        byte[] expectedHash = new byte[KeySize];
        Buffer.BlockCopy(src, 1 + SaltSize, expectedHash, 0, KeySize);

        byte[] actualHash;
        if (src[0] == 0x00)
        {
            // Legacy V1 format: 1,000 iterations, SHA-1
#pragma warning disable SYSLIB0041 // Type or member is obsolete
            using Rfc2898DeriveBytes bytes = new(cleanPassword, salt, IterationsV1);
            actualHash = bytes.GetBytes(KeySize);
#pragma warning restore SYSLIB0041
        }
        else if (src[0] == 0x01)
        {
            // Modern V2 format: 600,000 iterations, SHA-256
#if NET6_0_OR_GREATER
            actualHash = Rfc2898DeriveBytes.Pbkdf2(cleanPassword, salt, IterationsV2, HashAlgorithmName.SHA256, KeySize);
#else
            using var pbkdf2 = new Rfc2898DeriveBytes(cleanPassword, salt, IterationsV2, HashAlgorithmName.SHA256);
            actualHash = pbkdf2.GetBytes(KeySize);
#endif
        }
        else
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }

    public static string CipherPasswordReversible(string password, string keyBase64, string vectorBase64)
    {
        byte[] key = Convert.FromBase64String(keyBase64);
        byte[] initializationVector = Convert.FromBase64String(vectorBase64);

        if (password == null || password.Length <= 0)
        {
            throw new ArgumentNullException("The given message is null or empty");
        }

        if (key == null || key.Length <= 0)
        {
            throw new ArgumentNullException("The given key is null or empty");
        }

        if (initializationVector == null || initializationVector.Length <= 0)
        {
            throw new ArgumentNullException("The initialization vector is null or empty");
        }

        //The AES' instance is created and initialized
        byte[] encryptedTextAsBytesArray;
        using (Aes encryptionAlgorithmAESInstance = Aes.Create())
        {
            encryptionAlgorithmAESInstance.Key = key;
            encryptionAlgorithmAESInstance.IV = initializationVector;

            //An encryptor to perform the stream's transformation is created and the streams used for encryption are created also
            ICryptoTransform encryptor = encryptionAlgorithmAESInstance.CreateEncryptor(encryptionAlgorithmAESInstance.Key, encryptionAlgorithmAESInstance.IV);
            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new(cryptoStream))
            {
                //The message is written in the stream to be encrypted and the encrypted bytes array from the memory stream is returned
                streamWriter.Write(password);
            }
            encryptedTextAsBytesArray = memoryStream.ToArray();
        }

        return Convert.ToBase64String(encryptedTextAsBytesArray);
    }

    public static string DecipherPassword(string cipheredPasswordBase64, string keyBase64, string vectorBase64)
    {
        var encryptedTextAsBytesArray = Convert.FromBase64String(cipheredPasswordBase64);
        byte[] key = Convert.FromBase64String(keyBase64);
        byte[] initializationVector = Convert.FromBase64String(vectorBase64);

        if (encryptedTextAsBytesArray == null || encryptedTextAsBytesArray.Length <= 0)
        {
            throw new ArgumentNullException("cipherText");
        }
        if (key == null || key.Length <= 0)
        {
            throw new ArgumentNullException("The given key is null");
        }
        if (initializationVector == null || initializationVector.Length <= 0)
        {
            throw new ArgumentNullException("The initialization vector is null");
        }

        using Aes encryptionAlgorithmAESInstance = Aes.Create();
        encryptionAlgorithmAESInstance.Key = key;
        encryptionAlgorithmAESInstance.IV = initializationVector;

        ICryptoTransform decryptor = encryptionAlgorithmAESInstance.CreateDecryptor(encryptionAlgorithmAESInstance.Key, encryptionAlgorithmAESInstance.IV);
        using MemoryStream memoryStream = new(encryptedTextAsBytesArray);
        using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
        using StreamReader streamReader = new(cryptoStream);

        return streamReader.ReadToEnd();
    }

}