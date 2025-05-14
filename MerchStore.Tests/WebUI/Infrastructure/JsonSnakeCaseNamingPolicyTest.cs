using MerchStore.WebUI.Infrastructure;
using Xunit;

namespace MerchStore.Tests.WebUI.Infrastructure
{
    public class JsonSnakeCaseNamingPolicyTest
    {
        #region ConvertName

        [Theory]
        [InlineData("MyProperty", "my_property")]
        [InlineData("MyLongPropertyName", "my_long_property_name")]
        [InlineData("URL", "u_r_l")]
        [InlineData("IPAddress", "i_p_address")]
        [InlineData("OrderID", "order_i_d")]
        [InlineData("product", "product")]
        [InlineData("Product1", "product1")]
        [InlineData("ProductID2", "product_i_d2")]
        [InlineData("A", "a")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void ConvertName_ReturnsExpectedSnakeCase(string input, string expected)
        {
            // Arrange
            var policy = new JsonSnakeCaseNamingPolicy();

            // Act
            var result = policy.ConvertName(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion
    }
}