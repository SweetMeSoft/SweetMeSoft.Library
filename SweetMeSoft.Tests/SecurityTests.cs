using System;
using System.Security.Cryptography;
using SweetMeSoft.Tools;
using Xunit;

namespace SweetMeSoft.Tests;

public class SecurityTests
{
    [Fact]
    public void HashPasswordIrreversible_ShouldReturnValidBase64HashWithV2Marker()
    {
        // Arrange
        string password = "MySecurePassword123!";

        // Act
        string hashedPassword = Security.HashPasswordIrreversible(password);

        // Assert
        Assert.NotNull(hashedPassword);
        Assert.NotEmpty(hashedPassword);

        byte[] hashBytes = Convert.FromBase64String(hashedPassword);
        Assert.Equal(49, hashBytes.Length); // 1 byte marker + 16 bytes salt + 32 bytes key
        Assert.Equal(0x01, hashBytes[0]);   // Format marker V2 (600,000 PBKDF2 iterations with SHA256)
    }

    [Fact]
    public void VerifyHashedPasswordIrreversible_ShouldReturnTrueForCorrectPassword()
    {
        // Arrange
        string password = "MySecurePassword123!";
        string hashedPassword = Security.HashPasswordIrreversible(password);

        // Act
        bool isValid = Security.VerifyHashedPasswordIrreversible(hashedPassword, password);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyHashedPasswordIrreversible_ShouldReturnFalseForWrongPassword()
    {
        // Arrange
        string password = "MySecurePassword123!";
        string hashedPassword = Security.HashPasswordIrreversible(password);

        // Act
        bool isValid = Security.VerifyHashedPasswordIrreversible(hashedPassword, "WrongPassword");

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifyHashedPasswordIrreversible_ShouldVerifyLegacyV1HashSuccessfully()
    {
        // Arrange: Construct a legacy V1 hash (0x00 marker, 16-byte salt, 32-byte key generated with 1,000 PBKDF2 iterations)
        string password = "LegacyPassword456!";
        byte[] salt = new byte[16];
        new Random(42).NextBytes(salt);

        byte[] key;
#if NET6_0_OR_GREATER
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000, HashAlgorithmName.SHA1))
        {
            key = pbkdf2.GetBytes(32);
        }
#else
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000))
        {
            key = pbkdf2.GetBytes(32);
        }
#endif

        byte[] v1Payload = new byte[49];
        v1Payload[0] = 0x00; // Legacy marker V1
        Buffer.BlockCopy(salt, 0, v1Payload, 1, 16);
        Buffer.BlockCopy(key, 0, v1Payload, 17, 32);
        string base64V1Hash = Convert.ToBase64String(v1Payload);

        // Act
        bool isValid = Security.VerifyHashedPasswordIrreversible(base64V1Hash, password);
        bool isInvalidPassword = Security.VerifyHashedPasswordIrreversible(base64V1Hash, "WrongPassword");

        // Assert
        Assert.True(isValid);
        Assert.False(isInvalidPassword);
    }
}
