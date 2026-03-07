using HomeInventory.Client.Errors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;

namespace HomeInventory.Client.Services.Interfaces
{
    public class ErrorLocalizerService : IErrorLocalizer
    {
        private readonly ResourceManager _resourceManager = new("HomeInventory.Client.Resources.Strings", typeof(ErrorLocalizerService).Assembly);
        private CultureInfo _currentCulture = CultureInfo.CurrentCulture;

        public string GetString(string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            try
            {
                var result = _resourceManager.GetString(key, _currentCulture);
                return result ?? throw new KeyNotFoundException(nameof(key));
            } catch (MissingManifestResourceException)
            {
                throw new KeyNotFoundException(nameof(key));
            }
        }

        public void SetCulture(string cultureCode)
        {
            _currentCulture = new CultureInfo(cultureCode);
        }

        public string GetString(ApiErrorTypes errorType)
        {
            return errorType switch
            {
                ApiErrorTypes.Validation => GetString("ApiErrorValidation"),
                ApiErrorTypes.Unauthorized => GetString("ApiErrorUnauthorized"),
                ApiErrorTypes.Forbidden => GetString("ApiErrorForbidden"),
                ApiErrorTypes.NotFound => GetString("ApiErrorNotFound"),
                ApiErrorTypes.Conflict => GetString("ApiErrorConflict"),
                ApiErrorTypes.Server => GetString("ApiErrorServer"),
                ApiErrorTypes.Network => GetString("ApiErrorNetwork"),
                ApiErrorTypes.Unknown => GetString("ApiErrorUnknown"),
                _ => throw new KeyNotFoundException(),
            };
        }
    }
}
