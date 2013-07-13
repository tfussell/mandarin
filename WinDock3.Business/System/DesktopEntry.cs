using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using WinDock3.Service;
using IWshRuntimeLibrary;
using WindowsManagedApi;

namespace WinDock3.Business.System
{
    public class DesktopAction
    {
        public string Exec { get; set; }
        public string Name { get; set; }
    }

    public enum DesktopEntryType
    {
        Application,
        Link,
        Directory,
        Invalid
    }

    public class DesktopEntry
    {
        #region Static properties
        /// <summary>
        /// A minimal DesktopEntry to represent an invalid or uninitialized instance.
        /// </summary>
        private static DesktopEntry Invalid
        {
            get
            {
                return new DesktopEntry
                {
                    Version = new Version(1, 0),
                    Type = DesktopEntryType.Invalid
                };
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// The type of desktop entry. Currently one of: Application, Link, Directory.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public DesktopEntryType Type { get; set; }

        /// <summary>
        /// Version of the specification upon which this entry was built.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Specific name for the entry.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Generic name for the entry. In the case of "Google Chrome",
        /// the generic name might be "Web Browser".
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public string GenericName { get; set; }

        /// <summary>
        /// Description to be shown in a tooltip for this entry.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Name of an executable file which indicates if this entry is installed.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public string TryExec { get; set; }

        /// <summary>
        /// The command to execute the associated application.
        /// Usually this is an executable file followed by 0..N arguments.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public string Exec { get; set; }

        /// <summary>
        /// For an Application, the working directory in which it is to be executed.
        /// For a Directory, simply the full path to the directory.
        /// Note: This is not exactly specified by the spec.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Image file to be used to represent this item.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public Image Icon { get; set; }

        /// <summary>
        /// Mime types to be associated with this application.
        /// These are currently unused.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public List<string> MimeType { get; set; }

        /// <summary>
        /// A set of actions which may be called on this Application.
        /// These mirror the "Tasks" or verbs in a Windows JumpList.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public Dictionary<string, DesktopAction> Actions { get; set; }

        /// <summary>
        /// Menu categories which this application should be associated with.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public List<string> Categories { get; set; }

        /// <summary>
        /// If this is a link type, the URL to show when activated.
        /// See: https://developer.gnome.org/desktop-entry-spec/
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// A desktop entry file should have one of the these extensions.
        /// </summary>
        public static List<string> AllowedExtensions
        {
            get
            {
                return new List<string>
                    {
                        ".lnk",
                        ".desktop",
                        ".url",
                        ".directory",
                        ""
                    };
            }
        }

        #endregion

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
                            return Invalid;
                    }
                }

                return FromEntryFileMagic(entryFile);
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
            }

            return Invalid;
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

        private static DesktopEntry FromDirectoryEntryFile(string entryFile)
        {
            var contents = ParseEntryFileContents(entryFile);

            if (contents == null) return Invalid;

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
                    Icon = SystemService.GetSystemIcon(SystemService.SystemIconIndices.Directory)
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

            return Invalid;
        }

        private static DesktopEntry FromDesktopEntryFile(string entryFile)
        {
            var contents = ParseEntryFileContents(entryFile);

            if (contents == null) return Invalid;

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
                        Icon = tryExec != null && global::System.IO.File.Exists(tryExec) ? User32.ExtractIconW(tryExec, 0) : null
                    };
            }
            if(type == "Link")
            {
                return new DesktopEntry
                    {
                        Name = name,
                        Type = DesktopEntryType.Link,
                        Comment = comment,
                        URL = url
                    };
            }

            return Invalid;
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

            return Invalid;
        }

        public static DesktopEntry FromShellLinkFile(string linkFilePath)
        {
                var shellLink = GetShellLink(linkFilePath);
                var entryType = GetLinkType(shellLink);

                if (entryType == DesktopEntryType.Application)
                {
                    AppUserModelId appId = null;
                    try
                    {
                        AppUserModelId.Find(shellLink.TargetPath).FirstOrDefault();
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
                        icon = User32.ExtractIconW(shellLink.TargetPath, 0) ?? User32.ExtractIconW(linkFilePath, 0);
                    }
                    catch { }

                    return new DesktopEntry
                        {
                            Icon = icon,
                            TryExec = shellLink.TargetPath,
                            Exec = shellLink.TargetPath + " " + shellLink.Arguments,
                            Comment = shellLink.Description,
                            Name = global::System.IO.Path.GetFileNameWithoutExtension(shellLink.FullName),
                            Actions = actions,
                            Version = new Version(1, 0),
                            Path = shellLink.WorkingDirectory,
                            Type = DesktopEntryType.Application
                        };
                }
            if (entryType == DesktopEntryType.Directory)
            {
                return new DesktopEntry
                    {
                        Icon = SystemService.GetSystemIcon(SystemService.SystemIconIndices.Directory),
                        Name = global::System.IO.Path.GetFileNameWithoutExtension(shellLink.FullName),
                        Version = new Version(1, 0),
                        Path = shellLink.TargetPath,
                        Type = DesktopEntryType.Directory
                    };
            }

            return Invalid;
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

/*
        private string OldResolve(string lnkFile)
        {
            var shl = new Shell();
            var dir = shl.NameSpace(System.IO.Path.GetDirectoryName(lnkFile));
            var itm = dir.Items().Item(System.IO.Path.GetFileName(lnkFile));
            var lnk = itm.GetLink;

            if(lnk != null)
                lnk.Resolve(8);

            return lnk != null ? lnk.Path : ResolveShellLinkTargetMsi(lnkFile);
        }

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
