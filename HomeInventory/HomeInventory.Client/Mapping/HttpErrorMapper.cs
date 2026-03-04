using HomeInventory.Client.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Mapping
{
    public static class HttpErrorMapper
    {
        public static ApiException Map(HttpResponseMessage response, string? bode = null)
        {
            var code = (int)response.StatusCode;
            return code switch
            {
                400 => new ApiException(ApiErrorTypes.Validation, "Neplatný požadavek", code),
                401 => new ApiException(ApiErrorTypes.Unauthorized, "Nejste přihlášený", code),
                403 => new ApiException(ApiErrorTypes.Forbidden, "Nemáte oprávnění", code),
                404 => new ApiException(ApiErrorTypes.NotFound, "Položka nebyla nalezena", code),
                409 => new ApiException(ApiErrorTypes.Conflict, "Konflikt dat", code),
                500 => new ApiException(ApiErrorTypes.Server, "Chyba server", code),
                _ => new ApiException(ApiErrorTypes.Unknown, "Neočekávaná chyba", code)
            };
        }

        public static ApiException MapNetwork(Exception ex) => new(ApiErrorTypes.Network, "Nelze se připojit k serveru.", null, ex);
    }
}
