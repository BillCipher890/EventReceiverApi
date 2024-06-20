using EventReceiverApi.Web.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace EventReceiverApiTest
{
    public class ValidateEventParametersAttributeTests
    {
        private ActionExecutingContext CreateActionExecutingContext(Dictionary<string, object> actionArguments)
        {
            var httpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var routeData = new RouteData();
            var actionDescriptor = new Mock<Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor>();

            var actionContext = new ActionContext(httpContext.Object, routeData, actionDescriptor.Object, new ModelStateDictionary());
            return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), actionArguments, new Mock<Controller>().Object);
        }

        [Fact]
        public void OnActionExecuting_ShouldReturnBadRequest_WhenOffsetIsOutOfRange()
        {
            // Arrange
            var attribute = new ValidateEventParametersAttribute();
            var context = CreateActionExecutingContext(new Dictionary<string, object> { { "offset", 25 } });

            // Act
            attribute.OnActionExecuting(context);

            // Assert
            Assert.IsType<BadRequestObjectResult>(context.Result);
            var badRequestResult = context.Result as BadRequestObjectResult;
            Assert.True(badRequestResult.Value is SerializableError);
            var errors = badRequestResult.Value as SerializableError;
            Assert.True(errors.ContainsKey("offset"));
        }

        [Fact]
        public void OnActionExecuting_ShouldReturnBadRequest_WhenStartTimeIsGreaterThanOrEqualToEndTime()
        {
            // Arrange
            var attribute = new ValidateEventParametersAttribute();
            var context = CreateActionExecutingContext(new Dictionary<string, object>
            {
                { "startTime", DateTime.Now },
                { "endTime", DateTime.Now.AddHours(-1) }
            });

            // Act
            attribute.OnActionExecuting(context);

            // Assert
            Assert.IsType<BadRequestObjectResult>(context.Result);
            var badRequestResult = context.Result as BadRequestObjectResult;
            Assert.True(badRequestResult.Value is SerializableError);
            var errors = badRequestResult.Value as SerializableError;
            Assert.True(errors.ContainsKey("startTime"));
        }

        [Fact]
        public void OnActionExecuting_ShouldNotReturnBadRequest_WhenParametersAreValid()
        {
            // Arrange
            var attribute = new ValidateEventParametersAttribute();
            var context = CreateActionExecutingContext(new Dictionary<string, object>
            {
                { "offset", 10 },
                { "startTime", DateTime.Now },
                { "endTime", DateTime.Now.AddHours(1) }
            });

            // Act
            attribute.OnActionExecuting(context);

            // Assert
            Assert.Null(context.Result);
        }

        [Fact]
        public void OnActionExecuting_ShouldNotReturnBadRequest_WhenOffsetIsNotPresent()
        {
            // Arrange
            var attribute = new ValidateEventParametersAttribute();
            var context = CreateActionExecutingContext(new Dictionary<string, object>());

            // Act
            attribute.OnActionExecuting(context);

            // Assert
            Assert.Null(context.Result);
        }
    }
}
