using HomeInventory.Client.Errors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;

namespace HomeInventory.Client.Services.Interfaces
{
    public interface IErrorLocalizer
    {
        string GetString(ApiErrorTypes errorType);

        public string GetString(string key);

        public void SetCulture(string cultureCode);
    }
}
