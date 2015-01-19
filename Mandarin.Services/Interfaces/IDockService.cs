using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandarin.Business.Core;

namespace Mandarin.Services.Interfaces
{
    public interface IDockService
    {
        void GetDocks(Action<IEnumerable<Dock>, Exception> callback);
        void SaveDock(Dock dock, Action<bool> callback);
    }
}
