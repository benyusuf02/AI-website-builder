using Xunit;

namespace YDeveloper.Tests
{
    public class ExtensionTests
    {
        [Fact]
        public void ToSlug_ShouldConvertTurkishCharacters()
        {
            // Arrange
            var input = "Çiğdem Şehriye Öğrenci";
            
            // Act
            var result = input.ToSlug();
            
            // Assert
            Assert.Equal("cigdem-sehriye-ogrenci", result);
        }

        [Fact]
        public void IsValidEmail_ShouldReturnTrue_ForValidEmail()
        {
            // Arrange
            var email = "test@example.com";
            
            // Act
            var result = Utilities.ValidationUtility.IsValidEmail(email);
            
            // Assert
            Assert.True(result);
        }
    }
}
