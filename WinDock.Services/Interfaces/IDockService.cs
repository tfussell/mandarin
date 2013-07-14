using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinDock.Business.Core;

namespace WinDock.Services.Interfaces
{
    public interface IDockService
    {
        void GetDocks(Action<IEnumerable<Dock>, Exception> callback);
        void SaveDock(Dock dock, Action<bool> callback);
    }
}
