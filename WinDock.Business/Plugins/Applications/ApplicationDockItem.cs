using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WinDock.Business;
using WinDock.Business.Core;
using WindowsManagedApi.User32;

namespace WinDock.Plugins.Applications
{
    public class ApplicationDockItem : IconDockItem
    {
        public DesktopEntry DesktopEntry { get; set; }

        public bool Running
        {
            get { return WindowHandles.Any(); }
        }

        private string DisplayName { get; set; }
        private string ApplicationId { get; set; }
        public bool Pinned { get; set; }
        private bool Autorun { get; set; }
        private Dictionary<IntPtr, IntPtr> WindowHandles { get; set; }

        public ApplicationDockItem(IntPtr windowHandle)
        {
            Autorun = false;
            Pinned = false;

            var v = AppUserModelId.Find(windowHandle).Where(a => File.Exists(a.DestinationList));
            ApplicationId = v.Single().Id;
        }

        public ApplicationDockItem(DesktopEntry entry)
        {
            DesktopEntry = entry;

            WindowHandles = new Dictionary<IntPtr, IntPtr>();
            Autorun = false;
            Pinned = true;

            Name = entry.Name;

            Image = entry.Icon;

            if (Image != null)
            {
                ReflectionImage = CreateReflection(Image);
            }
        }

        protected override void OnLeftClick(object sender, EventArgs e)
        {
            LaunchOrShow();
        }

        public void RegisterWindowHandle(IntPtr processId, IntPtr windowHandle)
        {
            WindowHandles[processId] = windowHandle;
        }

        public void UnregisterWindowHandle(IntPtr processId)
        {
            WindowHandles.Remove(processId);
        }

        private void LaunchOrShow()
        {
            if (WindowHandles.Count == 0)
            {
                var p = Process.Start(DesktopEntry.Exec);

                if (p != null)
                {
                    Active = true;
                }
            }
            else
            {
                MoveToFront();
            }
        }

        private void MoveToFront()
        {
            foreach (var p in WindowHandles)
            {
                MoveToFront(p.Value);
            }
        }

        static void MoveToFront(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero) return;

            if (WindowsManagedApi.User32.Functions.IsIconic(windowHandle))
                WindowsManagedApi.User32.Functions.ShowWindow(windowHandle, (int)WindowsManagedApi.User32.Enumerations.ShowWindow.ShowNormal);

            WindowsManagedApi.User32.Functions.SetForegroundWindow(windowHandle);
        }

        private void ShowInExplorer()
        {
            Process.Start("explorer.exe", "/select, " + DesktopEntry.TryExec);
        }

        private void Quit()
        {
            foreach (var handle in WindowHandles)
            {
                Process.GetProcessById((int)handle.Key).CloseMainWindow();
            }

            Active = false;
        }

        public override IEnumerable<DockItemAction> MenuItems
        {
            get
            {
                if (Running)
                {
                    return new List<DockItemAction>
                    {
                        DockItemAction.CreateSubMenu("Options", new List<DockItemAction>
                            {
                                DockItemAction.CreateToggle("Keep in Dock", () => Pinned = true, () => Pinned = false, Pinned),
                                DockItemAction.CreateToggle("Open at Login", () => Autorun = true, () => Autorun = false, Autorun)
                            }),
                        DockItemAction.CreateNormal("Show in Explorer", ShowInExplorer),
                        null,
                        DockItemAction.CreateNormal("Show", MoveToFront),
                        DockItemAction.CreateNormal("Quit", Quit)
                    };
                }

                return new List<DockItemAction>
                {
                    DockItemAction.CreateSubMenu("Options", new List<DockItemAction>
                        {
                            DockItemAction.CreateToggle("Keep in Dock", () => Pinned = true, () => Pinned = false, Pinned),
                            DockItemAction.CreateToggle("Open at Login", () => Autorun = true, () => Autorun = false, Autorun)
                        }),
                    DockItemAction.CreateNormal("Show in Explorer", ShowInExplorer),
                    null,
                    DockItemAction.CreateNormal("Open", LaunchOrShow)
                };
            }
        }
    }
}
