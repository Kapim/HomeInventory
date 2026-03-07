using HomeInventory.Client.Errors;
using HomeInventory.Client.Http;
using HomeInventory.Client.Mapping;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System.Globalization;
using System.Net;
using System.Resources;

namespace HomeInventory.Client.Tests
{
    public class ErrorLocalizerTests
    {
        private readonly ErrorLocalizerService localizer = new();
        private readonly ResourceManager _resourceManager = new("HomeInventory.Client.Resources.Strings", typeof(ErrorLocalizerService).Assembly);

        [Fact]
        public async Task GetString_CorrectStringReturned()
        {
            string culture = CultureInfo.CurrentCulture.Name;
            localizer.SetCulture(culture);
            Assert.Equal(_resourceManager.GetString("ApiErrorValidation", CultureInfo.CurrentCulture), localizer.GetString(ApiErrorTypes.Validation));
            Assert.Equal(_resourceManager.GetString("ApiErrorUnauthorized", CultureInfo.CurrentCulture), localizer.GetString(ApiErrorTypes.Unauthorized));
            Assert.Equal(_resourceManager.GetString("ApiErrorForbidden", CultureInfo.CurrentCulture), localizer.GetString(ApiErrorTypes.Forbidden));
            Assert.Equal(_resourceManager.GetString("ApiErrorNotFound", CultureInfo.CurrentCulture), localizer.GetString(ApiErrorTypes.NotFound));
            Assert.Equal(_resourceManager.GetString("ApiErrorConflict", CultureInfo.CurrentCulture), localizer.GetString(ApiErrorTypes.Conflict));
            Assert.Equal(_resourceManager.GetString("ApiErrorServer", CultureInfo.CurrentCulture), localizer.GetString(ApiErrorTypes.Server));
            Assert.Equal(_resourceManager.GetString("ApiErrorUnknown", CultureInfo.CurrentCulture), localizer.GetString(ApiErrorTypes.Unknown));
        }

        [Fact]
        public async Task GetString_NotExistingCode_KeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => localizer.GetString((ApiErrorTypes)(-1)));
            Assert.Throws<KeyNotFoundException>(() => localizer.GetString("NotExistingKeyasdklfjasdf"));
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

        
        
    }
}
