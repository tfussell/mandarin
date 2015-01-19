using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mandarin.Services.Interfaces;
using Mandarin.Business.Core;
using Mandarin.Business.Settings;

namespace Mandarin.PresentationModel.Design
{
    class DesignDockService : IDockService
    {
        public void GetDocks(Action<IEnumerable<Dock>, Exception> callback)
        {
            var configuration = new DockConfiguration
            {
                ItemGroups = new List<string>
                {
                    "Applications",
                    null,
                    "RecycleBin"
                }
            };

            var dock = new Dock(configuration);

            callback(new List<Dock> { dock }, null);
        }


        public void SaveDock(Dock dock, Action<bool> callback)
        {
            throw new NotImplementedException();
        }
    }
}
