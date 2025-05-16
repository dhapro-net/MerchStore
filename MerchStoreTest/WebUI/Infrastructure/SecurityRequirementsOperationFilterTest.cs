using Xunit;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

using MerchStore.WebUI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using Moq;
using System.Collections.Generic;


namespace MerchStore.Tests.WebUI.Infrastructure
{
    public class SecurityRequirementsOperationFilterTest
    {
        // Group: Apply

        [Fact]
        public void Apply_AddsSecurity_WhenAuthorizeOnMethod()
        {
            // Arrange
            var operation = new OpenApiOperation();
            var context = TestHelper.CreateContextWithAuthorizeOnMethod();

            var filter = new SecurityRequirementsOperationFilter();

            // Act
            filter.Apply(operation, context);

            // Assert
            Assert.NotNull(operation.Security);
            Assert.Single(operation.Security);
        }

        [Fact]
        public void Apply_AddsSecurity_WhenAuthorizeOnController()
        {
            var operation = new OpenApiOperation();
            var context = TestHelper.CreateContextWithAuthorizeOnController();

            var filter = new SecurityRequirementsOperationFilter();

            filter.Apply(operation, context);

            Assert.NotNull(operation.Security);
            Assert.Single(operation.Security);
        }

        [Fact]
        public void Apply_DoesNotAddSecurity_WhenNoAuthorize()
        {
            var operation = new OpenApiOperation();
            var context = TestHelper.CreateContextWithoutAuthorize();

            var filter = new SecurityRequirementsOperationFilter();

            filter.Apply(operation, context);

            Assert.Null(operation.Security);
        }

        [Fact]
        public void Apply_DoesNotAddSecurity_WhenNotControllerAction()
        {
            var operation = new OpenApiOperation();
            var context = TestHelper.CreateContextNotControllerAction();

            var filter = new SecurityRequirementsOperationFilter();

            filter.Apply(operation, context);

            Assert.Null(operation.Security);
        }

        [Fact]
        public void Apply_DoesNotThrow_WhenMethodInfoIsNull()
        {
            var operation = new OpenApiOperation();
            var context = TestHelper.CreateContextWithNullMethodInfo();

            var filter = new SecurityRequirementsOperationFilter();

            var ex = Record.Exception(() => filter.Apply(operation, context));
            Assert.Null(ex);
            Assert.Null(operation.Security);
        }
    }

    // Helper class for creating mock OperationFilterContext objects
    public static class TestHelper
    {
        private static ISchemaGenerator MockSchemaGenerator() => new Mock<ISchemaGenerator>().Object;
        private static SchemaRepository MockSchemaRepository() => new SchemaRepository();

        public static OperationFilterContext CreateContextWithAuthorizeOnMethod()
        {
            var method = typeof(FakeController).GetMethod(nameof(FakeController.AuthorizedAction));
            return CreateContext(method);
        }

        public static OperationFilterContext CreateContextWithAuthorizeOnController()
        {
            var method = typeof(FakeAuthorizedController).GetMethod(nameof(FakeAuthorizedController.UnauthorizedAction));
            return CreateContext(method);
        }

        public static OperationFilterContext CreateContextWithoutAuthorize()
        {
            var method = typeof(FakeController).GetMethod(nameof(FakeController.UnauthorizedAction));
            return CreateContext(method);
        }

        public static OperationFilterContext CreateContextNotControllerAction()
        {
            // Simulate a non-controller action descriptor
            var apiDesc = new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription
            {
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            };

            return new OperationFilterContext(
                apiDesc,
                MockSchemaGenerator(),
                MockSchemaRepository(),
                null
            );
        }

        public static OperationFilterContext CreateContextWithNullMethodInfo()
        {
            var actionDescriptor = new ControllerActionDescriptor
            {
                ControllerTypeInfo = typeof(FakeController).GetTypeInfo(),
                MethodInfo = null
            };

            var apiDesc = new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription
            {
                ActionDescriptor = actionDescriptor
            };

            return new OperationFilterContext(
                apiDesc,
                MockSchemaGenerator(),
                MockSchemaRepository(),
                null
            );
        }

        private static OperationFilterContext CreateContext(MethodInfo methodInfo)
        {
            var controllerType = methodInfo?.DeclaringType;
            var actionDescriptor = new ControllerActionDescriptor
            {
                ControllerTypeInfo = controllerType?.GetTypeInfo(),
                MethodInfo = methodInfo
            };

            var apiDesc = new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription
            {
                ActionDescriptor = actionDescriptor
            };

            return new OperationFilterContext(
                apiDesc,
                MockSchemaGenerator(),
                MockSchemaRepository(),
                methodInfo
            );
        }
    }

    // Fake controllers for attribute testing
    public class FakeController
    {
        public void UnauthorizedAction() { }

        [Authorize]
        public void AuthorizedAction() { }
    }

    [Authorize]
    public class FakeAuthorizedController
    {
        public void UnauthorizedAction() { }
    }
}