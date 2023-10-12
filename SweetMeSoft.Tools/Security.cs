using System.Security.Cryptography;

namespace SweetMeSoft.Tools
{
    public class Security
    {
        public static string HashPasswordIrreversible(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        public static bool VerifyHashedPasswordIrreversible(string base64HashedPassword, string cleanPassword)
        {
            byte[] buffer4;
            if (base64HashedPassword == null)
            {
                return false;
            }
            if (cleanPassword == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(base64HashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new(cleanPassword, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
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

        private static bool ByteArraysEqual(byte[] firstHash, byte[] secondHash)
        {
            int _minHashLength = firstHash.Length <= secondHash.Length ? firstHash.Length : secondHash.Length;
            var xor = firstHash.Length ^ secondHash.Length;
            for (int i = 0; i < _minHashLength; i++)
                xor |= firstHash[i] ^ secondHash[i];
            return 0 == xor;
        }
    }
}
