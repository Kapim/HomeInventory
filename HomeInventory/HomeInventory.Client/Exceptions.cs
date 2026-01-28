using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client
{
    public sealed class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Neplatné přihlašovací údaje.") { }
    }

    public sealed class ApiUnavailableException : Exception
    {
        public ApiUnavailableException(string message, Exception? inner = null) : base(message, inner) { }
    }

}
