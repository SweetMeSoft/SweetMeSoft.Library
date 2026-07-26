using System;
using SweetMeSoft.Tools;
using Xunit;

namespace SweetMeSoft.Tests;

public class UtilsTests
{
    [Fact]
    public void MinifyJson_ShouldRemoveFormattingAndWhitespace()
    {
        // Arrange
        string indentedJson = @"
        {
            ""name"": ""SweetMeSoft"",
            ""version"": ""1.0.0"",
            ""active"": true
        }";

        // Act
        string minified = Utils.MinifyJson(indentedJson);

        // Assert
        Assert.NotNull(minified);
        Assert.DoesNotContain("\n", minified);
        Assert.DoesNotContain("  ", minified);
        Assert.Contains("\"name\":\"SweetMeSoft\"", minified);
    }

    [Fact]
    public void GetException_ShouldReturnFormattedExceptionMessage()
    {
        // Arrange
        var ex = new InvalidOperationException("Test exception message");

        // Act
        string details = Utils.GetException(ex);

        // Assert
        Assert.NotNull(details);
        Assert.Contains("Test exception message", details);
    }
}
