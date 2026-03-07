using HomeInventory.Client.Errors;
using HomeInventory.Client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Mapping
{
    public static class HttpErrorMapper
    {
        public static ApiException Map(HttpResponseMessage response, string? body = null)
        {
            var code = (int)response.StatusCode;
            if (code >= 500) return new ApiException(ApiErrorTypes.Server, body ?? string.Empty, code);
            return code switch
            {
                400 => new ApiException(ApiErrorTypes.Validation, body ?? string.Empty, code),
                401 => new ApiException(ApiErrorTypes.Unauthorized, body ?? string.Empty, code),
                403 => new ApiException(ApiErrorTypes.Forbidden, body ?? string.Empty, code),
                404 => new ApiException(ApiErrorTypes.NotFound, body ?? string.Empty, code),
                409 => new ApiException(ApiErrorTypes.Conflict, body ?? string.Empty, code),
                _ => new ApiException(ApiErrorTypes.Unknown, body ?? string.Empty, code)
            };
        }

        public static ApiException MapNetwork(Exception ex) => new(ApiErrorTypes.Network, string.Empty, null, ex);
    }
}
