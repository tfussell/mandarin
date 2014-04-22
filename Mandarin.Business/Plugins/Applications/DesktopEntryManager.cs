using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Drawing;
using IWshRuntimeLibrary;
using WinDock.Business.Core;
using System.Runtime.InteropServices;

namespace WinDock.Plugins.Applications
{
    public class DesktopEntryManager
    {
        //public event EventHandler<NotifyCollectionChangedEventArgs> DesktopEntriesChanged;

        // shorthand for all registered AND unregistered desktop items
        public IEnumerable<DesktopEntry> AllEntries
        {
            get { return RegisteredItems.Union(UnregisteredItems); }
        }

        List<DesktopEntry> RegisteredItems { get; set; }
        List<DesktopEntry> UnregisteredItems { get; set; }

        public static DesktopEntry FromFile(string entryFile)
        {
            try
            {
                var extension = global::System.IO.Path.GetExtension(entryFile);
                if (extension != null)
                {
                    extension = extension.ToLower();
                    switch (extension)
                    {
                        case ".desktop":
                            return FromDesktopEntryFile(entryFile);
                        case ".directory":
                            return FromDirectoryEntryFile(entryFile);
                        case ".lnk":
                            return FromShellLinkFile(entryFile);
                        case ".url":
                            return FromShellUrlFile(entryFile);
                        default:
                            return DesktopEntry.Invalid;
                    }
                }

                return FromEntryFileMagic(entryFile);
            }
            catch (Exception)
            {
                return DesktopEntry.Invalid;
            }
        }

        private static DesktopEntry FromShellUrlFile(string urlFilePath)
        {
            using (var reader = new StreamReader(new FileStream(urlFilePath, FileMode.Open)))
            {
                var line = reader.ReadLine();
                if (line != "[InternetShortcut]")
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        var split = line.Split('=');

                        if (split.Count() == 2 && split[0] == "URL")
                        {
                            var url = split[1];
                            return new DesktopEntry
                            {
                                URL = url,
                                Type = DesktopEntryType.Link,
                                Actions = new Dictionary<string, DesktopAction>(),
                                Categories = new List<string>(),
                                Name = global::System.IO.Path.GetFileNameWithoutExtension(urlFilePath),
                            };
                        }
                    }
                }
            }

            return DesktopEntry.Invalid;
        }

        private static DesktopEntry FromShellLinkFileAlt(string linkFilePath)
        {
            var shl = new Shell32.Shell();
            var dir = shl.NameSpace(System.IO.Path.GetDirectoryName(linkFilePath));
            var itm = dir.Items().Item(System.IO.Path.GetFileName(linkFilePath));
            var lnk = (Shell32.ShellLinkObject)itm.GetLink;

            if (lnk != null)
            {
                lnk.Resolve(0);
                var entryType = GetLinkTypeAlt(lnk);

                if (entryType == DesktopEntryType.Application)
                {
                    AppUserModelId appId = null;
                    try
                    {
                        appId = AppUserModelId.Find(lnk.Target.Path).FirstOrDefault();
                    }
                    catch { }
                    Dictionary<string, DesktopAction> actions = null;
                    if (appId != null && appId.DestinationList != null)
                    {
                        actions = ExtractJumpListActions(appId.DestinationList);
                    }

                    Image icon = null;

                    try
                    {
                        icon = WindowsManagedApi.User32.Helpers.ExtractIcon(lnk.Target.Path) ?? WindowsManagedApi.User32.Helpers.ExtractIcon(linkFilePath);
                    }
                    catch { }

                    if (icon == null)
                    {
                        icon = OldResolve(linkFilePath).ToBitmap();
                    }

                    return new DesktopEntry
                    {
                        Icon = icon,
                        TryExec = lnk.Target.Path,
                        Exec = lnk.Target.Path + " " + lnk.Arguments,
                        Comment = lnk.Description,
                        Name = global::System.IO.Path.GetFileNameWithoutExtension(linkFilePath),
                        Actions = actions,
                        Version = new Version(1, 0),
                        Path = Environment.ExpandEnvironmentVariables(lnk.WorkingDirectory),
                        Type = DesktopEntryType.Application
                    };
                }
                if (entryType == DesktopEntryType.Directory)
                {
                    return new DesktopEntry
                    {
                        Icon = WindowsManagedApi.User32.Helpers.ExtractIcon(Path.Combine(Environment.SystemDirectory, "..", "explorer.exe"), 0),
                        Name = global::System.IO.Path.GetFileNameWithoutExtension(linkFilePath),
                        Version = new Version(1, 0),
                        Path = lnk.Target.Path,
                        Type = DesktopEntryType.Directory
                    };
                }
            }

            return DesktopEntry.Invalid;
        }

        public static DesktopEntry FromShellLinkFile(string linkFilePath)
        {
            var shellLink = GetShellLink(linkFilePath);
            var entryType = GetLinkType(shellLink);

            if (entryType == DesktopEntryType.Invalid)
            {
                return FromShellLinkFileAlt(linkFilePath);
            }

            if (entryType == DesktopEntryType.Application)
            {
                AppUserModelId appId = null;
                try
                {
                   appId = AppUserModelId.Find(shellLink.TargetPath).FirstOrDefault();
                }
                catch { }
                Dictionary<string, DesktopAction> actions = null;
                if (appId != null && appId.DestinationList != null)
                {
                    actions = ExtractJumpListActions(appId.DestinationList);
                }

                Image icon = null;

                try
                {
                    icon = WindowsManagedApi.User32.Helpers.ExtractIcon(shellLink.TargetPath) ?? WindowsManagedApi.User32.Helpers.ExtractIcon(linkFilePath);
                }
                catch { }

                if (icon == null)
                {
                    icon = OldResolve(linkFilePath).ToBitmap();   
                }

                return new DesktopEntry
                {
                    Icon = icon,
                    TryExec = shellLink.TargetPath,
                    Exec = shellLink.TargetPath + " " + shellLink.Arguments,
                    Comment = shellLink.Description,
                    Name = global::System.IO.Path.GetFileNameWithoutExtension(shellLink.FullName),
                    Actions = actions,
                    Version = new Version(1, 0),
                    Path = Environment.ExpandEnvironmentVariables(shellLink.WorkingDirectory),
                    Type = DesktopEntryType.Application
                };
            }
            if (entryType == DesktopEntryType.Directory)
            {
                return new DesktopEntry
                {
                    Icon = WindowsManagedApi.User32.Helpers.ExtractIcon(Paths.SystemIconFile, 5),
                    Name = global::System.IO.Path.GetFileNameWithoutExtension(shellLink.FullName),
                    Version = new Version(1, 0),
                    Path = shellLink.TargetPath,
                    Type = DesktopEntryType.Directory
                };
            }

            return DesktopEntry.Invalid;
        }

        private static DesktopEntry FromDirectoryEntryFile(string entryFile)
        {
            var contents = ParseEntryFileContents(entryFile);

            if (contents == null) return DesktopEntry.Invalid;

            string name;
            contents.TryGetValue("Name", out name);
            string path;
            contents.TryGetValue("Path", out path);
            string icon;
            contents.TryGetValue("Icon", out icon);
            string comment;
            contents.TryGetValue("Comment", out comment);

            return new DesktopEntry
            {
                Name = name,
                Type = DesktopEntryType.Directory,
                Path = path,
                Comment = comment,
                Icon = WindowsManagedApi.User32.Helpers.ExtractIcon(Paths.SystemIconFile, 5)
            };
        }

        // XXX: No reason to need to parse the file twice.
        private static DesktopEntry FromEntryFileMagic(string entryFile)
        {
            var contents = ParseEntryFileContents(entryFile);

            if (contents["Type"] == "Directory")
            {
                return FromDirectoryEntryFile(entryFile);
            }

            if (contents["Type"] == "Application" || contents["Type"] == "Link")
            {
                return FromDirectoryEntryFile(entryFile);
            }

            return DesktopEntry.Invalid;
        }

        private static DesktopEntry FromDesktopEntryFile(string entryFile)
        {
            var contents = ParseEntryFileContents(entryFile);

            if (contents == null) return DesktopEntry.Invalid;

            string name;
            contents.TryGetValue("Name", out name);
            string path;
            contents.TryGetValue("Path", out path);
            string icon;
            contents.TryGetValue("Icon", out icon);
            string comment;
            contents.TryGetValue("Comment", out comment);
            string url;
            contents.TryGetValue("URL", out url);
            string exec;
            contents.TryGetValue("Exec", out exec);
            string tryExec;
            contents.TryGetValue("TryExec", out tryExec);

            var type = contents["Type"];

            if (type == "Application")
            {
                return new DesktopEntry
                {
                    Name = name,
                    Type = DesktopEntryType.Application,
                    Path = path,
                    Comment = comment,
                    Exec = exec,
                    TryExec = tryExec,
                    Icon = tryExec != null && global::System.IO.File.Exists(tryExec) ? WindowsManagedApi.User32.Helpers.ExtractIcon(tryExec, 0) : null
                };
            }
            if (type == "Link")
            {
                return new DesktopEntry
                {
                    Name = name,
                    Type = DesktopEntryType.Link,
                    Comment = comment,
                    URL = url
                };
            }

            return DesktopEntry.Invalid;
        }

        private static Dictionary<string, string> ParseEntryFileContents(string entryFile)
        {
            var contents = new Dictionary<string, string>();

            using (var reader = new StreamReader(new FileStream(entryFile, FileMode.Open)))
            {
                while (reader.ReadLine() != "[Desktop Entry]" && !reader.EndOfStream)
                {
                }

                if (reader.EndOfStream) return null;

                string line = reader.ReadLine();
                if (line != null)
                {
                    while (!String.IsNullOrEmpty(line))
                    {
                        var split = line.Split('=');
                        if (split.Count() != 2)
                        {
                            throw new Exception("Bad DesktopEntry file at line: " + line);
                        }
                        contents[split[0]] = split[1];
                        if (reader.EndOfStream) break;
                        line = reader.ReadLine();
                    }
                }
            }

            return contents;
        }

        private static IWshShortcut GetShellLink(string linkFilePath)
        {
            if (!global::System.IO.File.Exists(linkFilePath))
            {
                throw new FileNotFoundException("Tried to create DesktopEntry from non-existent lnk file: " + linkFilePath);
            }

            IWshShell shell = new IWshShell_Class();
            var shellLink = (IWshShortcut)shell.CreateShortcut(linkFilePath);

            return shellLink;
        }

        private static DesktopEntryType GetLinkTypeAlt(Shell32.ShellLinkObject shellLink)
        {
            var name = shellLink.Target.Name;
            var type = shellLink.Target.Type;

            if (name == "File Explorer" && type == "System Folder")
            {
                return DesktopEntryType.Directory;
            }

            var extension = global::System.IO.Path.GetExtension(shellLink.Target.Path);

            if (!string.IsNullOrEmpty(extension))
            {
                if (extension.ToLower() == ".exe" && global::System.IO.File.Exists(shellLink.Target.Path))
                {
                    return DesktopEntryType.Application;
                }
            }
            else if (extension == "" && Directory.Exists(shellLink.Target.Path))
            {
                return DesktopEntryType.Directory;
            }

            return DesktopEntryType.Invalid;
        }

        private static DesktopEntryType GetLinkType(IWshShortcut shellLink)
        {
            if (shellLink == null || shellLink.TargetPath == null)
            {
                throw new Exception("Unable to parse lnk file. It may be malformed or corrupt.");
            }

            var extension = global::System.IO.Path.GetExtension(shellLink.TargetPath);

            if (!string.IsNullOrEmpty(extension))
            {
                if (extension.ToLower() == ".exe" && global::System.IO.File.Exists(shellLink.TargetPath))
                {
                    return DesktopEntryType.Application;
                }
            }
            else if (extension == "" && Directory.Exists(shellLink.TargetPath))
            {
                return DesktopEntryType.Directory;
            }

            return DesktopEntryType.Invalid;
        }

        private static Dictionary<string, DesktopAction> ExtractJumpListActions(string destinationList)
        {
            var actions = new Dictionary<string, DesktopAction>();

            using (var reader = new BinaryReader(new FileStream(destinationList, FileMode.Open)))
            {
                reader.ReadByte();
            }

            return actions;
        }

        private static Icon OldResolve(string lnkFile)
        {
            var shl = new Shell32.Shell();
            var dir = shl.NameSpace(System.IO.Path.GetDirectoryName(lnkFile));
            var itm = dir.Items().Item(System.IO.Path.GetFileName(lnkFile));
            var lnk = (Shell32.ShellLinkObject)itm.GetLink;

            if (lnk != null)
            {
                string iconLocation;
                var index = lnk.GetIconLocation(out iconLocation);
                iconLocation = Environment.ExpandEnvironmentVariables(iconLocation);
                return Extract(iconLocation, index, true);
            }

            return null;
        }

        public static Icon Extract(string file, int number, bool largeIcon)
        {
            IntPtr large;
            IntPtr small;
            ExtractIconEx(file, number, out large, out small, 1);
            try
            {
                return Icon.FromHandle(largeIcon ? large : small);
            }
            catch
            {
                return null;
            }

        }

        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

        /*
        private static string ResolveShellLinkTargetMsi(string lnkFile)
        {
            String product;
            String feature;
            String component;

            if (Msi.MsiGetShortcutTargetW(lnkFile, out product, out feature, out component))
            {
                String path;

                var installState = Msi.MsiGetComponentPathW(product, component, out path);
                return installState == Msi.InstallState.Local ? path : null;
            }

            return null;
        }
        */
    }
}
