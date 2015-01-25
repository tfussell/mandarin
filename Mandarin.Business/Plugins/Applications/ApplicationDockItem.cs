using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mandarin.Business;
using Mandarin.Business.Core;
using WindowsManagedApi.User32;
using System.Runtime.InteropServices;

namespace Mandarin.Plugins.Applications
{
    public class ApplicationDockItem : IconDockItem
    {
        public DesktopEntry DesktopEntry { get; set; }

        public bool Running
        {
            get { return WindowHandles.Any(); }
        }

        private string DisplayName { get; set; }
        public AppUserModelId AppId { get; set; }
        public bool Pinned { get; set; }
        private bool Autorun { get; set; }
        private List<IntPtr> WindowHandles { get; set; }

        public ApplicationDockItem(IntPtr windowHandle)
        {
            WindowHandles = new List<IntPtr> { windowHandle };

            Autorun = false;
            Pinned = false;

            var possibleIds = AppUserModelId.Find(windowHandle);
            AppId = possibleIds.Where(a => File.Exists(a.DestinationList)).FirstOrDefault();
            if(AppId == null)
            {
                AppId = possibleIds.FirstOrDefault();
            }

            if (AppId != null)
            {
                DesktopEntry = new DesktopEntry()
                {
                    TryExec = AppId.Executable
                };
            }
        }

        public ApplicationDockItem(DesktopEntry entry)
        {
            WindowHandles = new List<IntPtr>();

            DesktopEntry = entry;

            Autorun = false;
            Pinned = true;

            Name = entry.Name;

            Image = entry.Icon;

            if (entry.Type == DesktopEntryType.Application)
            {
                var possitiblities = AppUserModelId.Find(entry.TryExec);
                AppId = AppUserModelId.Find(entry.TryExec).Where(a => File.Exists(a.DestinationList)).FirstOrDefault();
                if(AppId == null)
                {
                    AppId = possitiblities.FirstOrDefault();
                }
            }
            else
            {
                AppId = AppUserModelId.FromExplicitAppId("C:\\Windows\\explorer.exe", "Microsoft.Windows.Explorer").First();
            }
        }

        public bool HasRegisteredWindowHandle(IntPtr hWnd)
        {
            return WindowHandles.Contains(hWnd);
        }

        protected override void OnLeftClick(object sender, EventArgs e)
        {
            LaunchOrShow();
        }

        public void RegisterWindowHandle(IntPtr hWnd)
        {
            WindowHandles.Add(hWnd);
            Active = true;
        }

        public void UnregisterWindowHandle(IntPtr hWnd)
        {
            WindowHandles.Remove(hWnd);
            if (!Running) Active = false;
        }

        private void LaunchOrShow()
        {
            if (WindowHandles.Count == 0)
            {
                Process p = null;

                if (DesktopEntry.Type == DesktopEntryType.Application)
                {
                    var extensionIndex = DesktopEntry.Exec.IndexOf(".exe");
                    var exe = DesktopEntry.Exec.Substring(0, extensionIndex + 4);
                    var args = DesktopEntry.Exec.Substring(extensionIndex + 5);
                    p = Process.Start(exe, args);
                }
                else if (DesktopEntry.Type == DesktopEntryType.Directory)
                {
                    p = Process.Start("explorer.exe", "/e,shell:" + DesktopEntry.Path);
                }

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
                if (p != IntPtr.Zero)
                {
                    MoveToFront(p);
                }
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

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private void Quit()
        {
            foreach (var handle in WindowHandles)
            {
                uint processId = 0;
                GetWindowThreadProcessId(handle, out processId);
                Process.GetProcessById((int)processId).CloseMainWindow();
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
