using MerchStore.WebUI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MerchStore.Tests.WebUI.Controllers
{
    public class LandingControllerTest
    {
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var controller = new LandingController();

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Default view
        }
    }
}