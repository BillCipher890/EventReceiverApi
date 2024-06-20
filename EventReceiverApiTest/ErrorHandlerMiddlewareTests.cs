using EventReceiverApi.Web.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;
using System.Text.Json;

namespace EventReceiverApiTest
{
    public class ErrorHandlerMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly ErrorHandlerMiddleware _middleware;

        public ErrorHandlerMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _middleware = new ErrorHandlerMiddleware(_nextMock.Object);
        }

        [Fact]
        public async Task Invoke_ShouldCallNextDelegate()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            await _middleware.Invoke(context);

            // Assert
            _nextMock.Verify(next => next(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_ShouldHandleExceptionAndReturnInternalServerError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exceptionMessage = "Test exception";
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(new Exception(exceptionMessage));

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Act
            await _middleware.Invoke(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(response);
            Assert.Equal(exceptionMessage, responseObject["error"]);

            // Restore the original response body stream
            context.Response.Body = originalBodyStream;
        }


        [Fact]
        public async Task Invoke_ShouldSetContentTypeToApplicationJsonOnError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(new Exception("Test exception"));

            // Act
            await _middleware.Invoke(context);

            // Assert
            Assert.Equal("application/json", context.Response.ContentType);
        }

        [Fact]
        public async Task Invoke_ShouldNotAlterResponseOnSuccess()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Act
            await _middleware.Invoke(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
            Assert.Equal(string.Empty, response);

            // Restore the original response body stream
            context.Response.Body = originalBodyStream;
        }
    }
}
