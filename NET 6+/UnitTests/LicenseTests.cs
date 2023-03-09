namespace SoftwareCopyrightManagementSystem.UnitTests
{
    [TestFixture]
    public class LicenseTests
    {
        [Test]
        public async Task Check_ValidLicense_ReturnsTrue()
        {
            // Arrange
            License license = new License();
            
            // Act
            var result = await license.Check("webapp", "SoftwareCopyrightManagementSystem", "1.0");
            
            // Assert
            Assert.IsTrue(result.Item1);
            Assert.AreEqual("paid", result.Item2);
        }
    }
}
