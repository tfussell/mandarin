using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinDock3.Presentation.ViewModel;

namespace WinDock3.Presentation.DataService
{
    public interface IDataService
    {
        void GetDocks(Action<IEnumerable<DockViewModel>, Exception> callback);
        void SaveDock(DockViewModel toSave, Action<bool> callback);
    }
}
