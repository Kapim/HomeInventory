using HomeInventory.Client.Errors;
using HomeInventory.Client.Http;
using HomeInventory.Client.Mapping;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System.Net;

namespace HomeInventory.Client.Tests
{
    public class HttpErrorMapperTests
    {
        [Fact]
        public async Task Map_CorrectErrorTypeReturned()
        {
            TestMapper(HttpStatusCode.BadRequest, ApiErrorTypes.Validation, 400);
            TestMapper(HttpStatusCode.Unauthorized, ApiErrorTypes.Unauthorized, 401);
            TestMapper(HttpStatusCode.Forbidden, ApiErrorTypes.Forbidden, 403);
            TestMapper(HttpStatusCode.NotFound, ApiErrorTypes.NotFound, 404);
            TestMapper(HttpStatusCode.Conflict, ApiErrorTypes.Conflict, 409);
            TestMapper(HttpStatusCode.InternalServerError, ApiErrorTypes.Server, 500);
            TestMapper(HttpStatusCode.LoopDetected, ApiErrorTypes.Server, 508);
        }

        [Fact]
        public async Task MapNetwork_CorrectErrorTypeReturned()
        {
            var exception = new Exception("TestError");
            var result = HttpErrorMapper.MapNetwork(exception);
            var expected = new ApiException(ApiErrorTypes.Network, string.Empty, null, exception);
            Assert.Equal(expected.Type, result.Type);
            Assert.Equal(expected.Message, result.Message);
            Assert.Equal(expected.StatusCode, result.StatusCode);
            Assert.Equal(expected.InnerException, result.InnerException);
        }

        private static void TestMapper(HttpStatusCode httpStatusCode, ApiErrorTypes apiErrorType, int statusCode)
        {
            var result = HttpErrorMapper.Map(new(httpStatusCode));
            var expected = new ApiException(apiErrorType, string.Empty, statusCode);
            Assert.Equal(expected.Type, result.Type);
            Assert.Equal(expected.Message, result.Message);
            Assert.Equal(expected.StatusCode, result.StatusCode);
        }
        
    }
}
