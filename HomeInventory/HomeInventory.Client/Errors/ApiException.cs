using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Errors
{
    public class ApiException(ApiErrorTypes type, string message, int? statusCode = null, Exception? inner = null) : Exception(message, inner)
    {
        public ApiErrorTypes Type { get; } = type;
        public int? StatusCode { get; } = statusCode;
    }
}
