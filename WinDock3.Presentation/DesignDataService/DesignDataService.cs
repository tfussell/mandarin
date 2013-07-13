using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinDock3.Business;
using WinDock3.Presentation.ViewModel;
using WinDock3.Presentation.DataService;
using System.IO;
using WinDock3.Business.Dock;
using WinDock3.Business.Settings;

namespace WinDock3.Presentation.DesignDataService
{
    public class DesignDataService : IDataService
    {
        public void GetDocks(Action<IEnumerable<DockViewModel>, Exception> callback)
        {
            var docks = new List<DockViewModel>();

            var model = new Dock(new DockConfiguration());
            var dock = new DockViewModel(model);
            docks.Add(dock);

            callback.Invoke(docks, null);
        }

        public void SaveDock(DockViewModel toSave, Action<bool> callback)
        {
            callback.Invoke(true);
        }
    }
}
