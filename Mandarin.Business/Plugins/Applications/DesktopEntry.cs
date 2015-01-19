using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using IWshRuntimeLibrary;

namespace Mandarin.Plugins.Applications
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
        /// <summary>
        /// A minimal DesktopEntry to represent an invalid or uninitialized instance.
        /// </summary>
        public static DesktopEntry Invalid
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
    }
}
