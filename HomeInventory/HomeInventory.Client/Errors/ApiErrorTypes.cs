using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Errors
{
    public enum ApiErrorTypes
    {
        Validation, Unauthorized, Forbidden, NotFound, Conflict, Server, Network, InvalidResponse, Unknown
    }
}
