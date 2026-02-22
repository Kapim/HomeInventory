using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Desktop.Wpf.Services
{
    public interface IAsyncInitializable
    {
        Task InitializeAsync(CancellationToken ct = default);
    }
}
