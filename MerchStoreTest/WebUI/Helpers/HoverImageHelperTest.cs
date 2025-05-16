using MerchStore.WebUI.Helpers;
using Xunit;

namespace MerchStoreTest.WebUI.Helpers
{
    public class HoverImageHelperTest
    {
        #region GetHoverImageUrl

        [Theory]
        [InlineData("Litter cute Mug", "/img/merchimug02.png")]
        [InlineData("litter cute mug", "/img/merchimug02.png")]
        [InlineData("LITTER CUTE MUG", "/img/merchimug02.png")]
        [InlineData("Laptop Sticker Pack", "/img/sticker01.png")]
        [InlineData("laptop sticker pack", "/img/sticker01.png")]
        [InlineData("Hoodie", "/img/hoodie.png")]
        [InlineData("hoodie", "/img/hoodie.png")]
        [InlineData("Canvas", "/img/canvas01.png")]
        [InlineData("Unknown Product", "/img/canvas01.png")]
        [InlineData("", "/img/canvas01.png")]
        [InlineData(null, "/img/canvas01.png")]
        public void GetHoverImageUrl_ReturnsExpectedUrl(string productName, string expectedUrl)
        {
            // Act
            var result = HoverImageHelper.GetHoverImageUrl(productName ?? string.Empty);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        #endregion
    }
}