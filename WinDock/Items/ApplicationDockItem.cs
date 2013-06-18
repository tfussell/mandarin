using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WinDock.GUI;
using WinDock.Services;
using WindowsShellFacade;
using Shortcut = WinDock.Services.Shortcut;

namespace WinDock.Items
{
    internal class ApplicationDockItem : IconDockItem
    {
        public event EventHandler ContextMenuRequested;

        private string DisplayName { get; set; }
        private string ApplicationId { get; set; }
        private string ExecutablePath { get; set; }
        private string FriendlyProcessName { get; set; }
        private bool Pinned { get; set; }
        private bool Autorun { get; set; }
        public bool Running { get; private set; }
        private Dictionary<IntPtr, IntPtr> WindowHandles { get; set; }

        public ApplicationDockItem(IntPtr windowHandle)
        {
            var processId = (int) User32.GetWindowThreadProcessId(windowHandle);
            var process = Process.GetProcessById(processId);
            WindowHandles = new Dictionary<IntPtr, IntPtr> {{new IntPtr(processId), windowHandle}};
            ExecutablePath = process.MainModule.FileName;
            Autorun = false;
            Pinned = false;

            var v = AppId.Find(windowHandle).Where(a => File.Exists(a.DestinationList));
            ApplicationId = v.Single().AppUserModelId;

        }

        public ApplicationDockItem(string filename)
        {
            WindowHandles = new Dictionary<IntPtr, IntPtr>();
            ExecutablePath = filename;
            Autorun = false;
            Pinned = true;

            if (IsShortcut(filename))
            {
                DisplayName = Path.GetFileNameWithoutExtension(filename);
                var referent = Shortcut.ParseShortcut(filename);
                FriendlyProcessName = Path.GetFileNameWithoutExtension(referent);
                if (referent != null)
                    Image = User32.ExtractIconW(referent) ?? User32.ExtractIconW(filename);
            }
            else
            {
                FriendlyProcessName = Path.GetFileNameWithoutExtension(filename);
                DisplayName = Path.GetFileNameWithoutExtension(filename);
            }

            var processes = Process.GetProcessesByName(FriendlyProcessName).Where(p => p.MainWindowHandle != IntPtr.Zero).ToList();
            if (processes.Any())
            {
                Running = true;
                var v = AppId.Find(processes[0].MainWindowHandle).Where(a => File.Exists(a.DestinationList));
                if(v.Any())
                    ApplicationId = v.Single().AppUserModelId;
            }

            Name = DisplayName;

            if (Image == null)
            {
                var associatedIcon = Icon.ExtractAssociatedIcon(filename);
                if (associatedIcon != null)
                    Image = associatedIcon.ToBitmap();
            }

            if (Image != null)
            {
                ReflectionImage = CreateReflection(Image);
            }
        }

        protected override void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LaunchOrShow();
            }
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenuRequested(sender, new EventArgs());
            }
        }

        public void AddWindow(IntPtr processId, IntPtr windowHandle)
        {
            WindowHandles[processId] = windowHandle;
        }

        public void RemoveWindow(IntPtr processId)
        {
            WindowHandles.Remove(processId);
        }

        private void LaunchOrShow()
        {
            if (WindowHandles.Count == 0)
            {
                var p = Process.Start(ExecutablePath);
                if (p != null)
                {
                    Running = true;
                    OnImageChange(Image);
                }
            }
            else
            {
                foreach (var p in WindowHandles)
                {
                    MoveToFront(p.Value);
                }
            }
        }

        private static bool IsShortcut(string filename)
        {
            return Path.GetExtension(filename.ToLower()) == ".lnk";
        }

        static void MoveToFront(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
                return;

            if (windowHandle == IntPtr.Zero)
                return;

            if (User32.IsIconic(windowHandle))
                User32.ShowWindow(windowHandle, User32.Sw.ShowNormal);

            User32.SetForegroundWindow(windowHandle);
        }

        public override void AddRightClickMenuItems(RightClickMenu rightClickMenu)
        {
            rightClickMenu.AddToggleItem("Keep in Dock", "Keep in Dock", () => Pinned = true, () => Pinned = false, Pinned);
            rightClickMenu.AddToggleItem("Open at Login", "Open at Login", () => Autorun = true, () => Autorun = false, Autorun);
            rightClickMenu.AddTextItem("Show in Explorer", ShowInExplorer);

            rightClickMenu.AddSeparatorItem();

            if (Running)
            {
                rightClickMenu.AddTextItem("Show", ShowInExplorer);
                rightClickMenu.AddTextItem("Quit", Quit);
            }
            else
            {
                rightClickMenu.AddTextItem("Open", () => Process.Start(ExecutablePath));
            }
        }

        private void ShowInExplorer()
        {
            Process.Start("explorer.exe", "/select, "+Shortcut.ParseShortcut(ExecutablePath));
        }

        private void Quit()
        {
            foreach (var handle in WindowHandles)
            {
                Process.GetProcessById((int)handle.Key).CloseMainWindow();
            }
            Running = false;
            OnImageChange(Image);
        }
    }
}
