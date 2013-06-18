using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WindowsShellFacade;

namespace WinDock.Services
{
    internal class AppId
    {
        private const string AutomaticDestinationsJumplistFolder =
            "%appdata%\\Microsoft\\Windows\\Recent\\automaticDestinations";

        private const string CustomDestinationsJumplistFolder =
            "%appdata%\\Microsoft\\Windows\\Recent\\customDestinations";

        private const ulong CrcPolynomial = 0x92C64265D32139A4;

        private static readonly ulong[] CrcTable = new ulong[256];

        private static readonly Dictionary<string, string> SystemDependentKnownFolderIds = new Dictionary<string, string>
            {
                {"FOLDERID_System", "{1AC14E77-02E7-4E5D-B744-2EB1AE5198B7}"},
                {"FOLDERID_ProgramFilesCommonX86", "{DE974D24-D9C6-4D3E-BF91-F4455120B917}"},
                {"FOLDERID_ProgramFilesCommon", "{F7F1ED05-9F6D-47A2-AAAE-29D317C6F066}"},
                {"FOLDERID_ProgramFilesCommonX64", "{6365D5A7-0F0D-45E5-87F6-0DA56B6A4F7D}"},
                {"FOLDERID_ProgramFiles", "{905e63b6-c1bf-494e-b29c-65b732d3d21a}"},
                {"FOLDERID_ProgramFilesX86", "{7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E}"},
                {"FOLDERID_SystemX86", "{D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27}"},
                {"FOLDERID_Windows", "{F38BF404-1D43-42F2-9305-67DE0B28FC23}"},
                {"FOLDERID_ProgramFilesX64", "{6D809377-6AF0-444b-8957-A3773F02200E}"}
            };

        private static readonly Dictionary<string, string> CommonKnownFolderIds = new Dictionary<string, string>
            {
                {
                    "%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\Administrative Tools",
                    "{724EF170-A42D-4FEF-9F26-B60E846FBA4F}"
                },
                {"%SystemDrive%\\Users\\Public", "{DFDF76A2-C82A-4D63-906A-5644AC457385}"},
                {"%SystemDrive%\\Users\\%USERNAME%", "{5E6C858F-0E22-4760-9AFE-EA3317B67173}"},
                {"%PUBLIC%\\Pictures\\Sample Pictures", "{C4900540-2379-4C75-844B-64E6FAF8716B}"},
                {"%windir%", "{F38BF404-1D43-42F2-9305-67DE0B28FC23}"},
                {"%windir%\\resources\\0409", "{2A00375E-224C-49DE-B8D1-440DF7EF3DDC}"},
                {"%PUBLIC%\\Downloads", "{3D644C9B-1FB8-4f30-9B45-F670235F79C0}"},
                {
                    "%APPDATA%\\Microsoft\\Windows\\Libraries\\Documents.library-ms",
                    "{7B0DB17D-9CD2-4A93-9733-46CC89022E7C}"
                },
                {
                    "%USERPROFILE%\\AppData\\Local\\Microsoft\\Windows\\Application Shortcuts",
                    "{A3918781-E5F2-4890-B3D9-A7E54332328C}"
                },
                {"%PUBLIC%\\Pictures", "{B6EBFB86-6907-413C-9AF7-4FC2ABF07CC5}"},
                {"%LOCALAPPDATA%\\Programs\\Common", "{BCBD3057-CA5C-4622-B42D-BC56DB0AE516}"},
                {"%LOCALAPPDATA%", "{F1B32785-6FBA-4FCF-9D55-7B8E7F157091}"},
                {"%ALLUSERSPROFILE%\\Microsoft\\Windows\\Libraries", "{48DAF80B-E6CF-4F4E-B800-0E69D84EE384}"},
                {"%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs", "{A77F5D77-2E2B-44C3-A6A2-ABA601054A51}"},
                {"%USERPROFILE%\\Favorites", "{1777F761-68AD-4D8A-87BD-30B759FA33DD}"},
                {"%PUBLIC%\\RecordedTV.library-ms", "{1A6FDBA2-F42D-4358-A798-B74D745926C5}"},
                {"%APPDATA%\\Microsoft\\Internet Explorer\\Quick Launch", "{52a4f021-7b75-48a9-9f6b-4b87a210bc8f}"},
                {"%USERPROFILE%\\Desktop", "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}"},
                {
                    "%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\StartUp",
                    "{B97D20BB-F46A-4C97-BA10-5E3608430854}"
                },
                {"%APPDATA%\\Microsoft\\Windows\\Recent", "{AE50C081-EBD2-438A-8655-8A092E34987A}"},
                {
                    "%LOCALAPPDATA%\\Microsoft\\Windows\\Temporary Internet Files",
                    "{352481E8-33BE-4251-BA85-6007CAEDCF9D}"
                },
                {"%APPDATA%\\Microsoft\\Windows\\Cookies", "{2B0F765D-C0E9-4171-908E-08A611B84FF6}"},
                {"%USERPROFILE%\\Links", "{bfb9d5e0-c6a9-404c-b2b2-ae6db6af4968}"},
                {"%ALLUSERSPROFILE%\\Microsoft\\Windows\\DeviceMetadataStore", "{5CE4A5E9-E4EB-479D-B89F-130C02886155}"},
                {"%PUBLIC%", "{DFDF76A2-C82A-4D63-906A-5644AC457385}"},
                {"%LOCALAPPDATA%\\Microsoft\\Windows\\GameExplorer", "{054FAE61-4DD8-4787-80B6-090220C4B700}"},
                {"%USERPROFILE%\\AppData\\LocalLow", "{A520A1A4-1780-4FF6-BD18-167343C5AF16}"},
                {"%USERPROFILE%\\AppData\\Local", "{F1B32785-6FBA-4FCF-9D55-7B8E7F157091}"},
                {"%APPDATA%\\Microsoft\\Windows\\Libraries", "{1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE}"},
                {"%windir%\\Resources", "{8AD10C31-2ADB-4296-A8F7-E4701232C972}"},
                {"%PUBLIC%\\Desktop", "{C4AA340D-F20F-4863-AFEF-F87EF2E6BA25}"},
                {"%APPDATA%\\Microsoft\\Windows\\Printer Shortcuts", "{9274BD8D-CFD1-41C3-B35E-B13F55A758F4}"},
                {"%USERPROFILE%\\Searches", "{7d1d3a04-debb-4115-95cf-2f29da2920da}"},
                {"%PUBLIC%\\Videos", "{2400183A-6185-49FB-A2D8-4A392A602BA3}"},
                {"%LOCALAPPDATA%\\Programs", "{5CD7AEE2-2219-4A67-B85D-6C9CE15660CB}"},
                {"%APPDATA%\\Microsoft\\Windows\\Start Menu", "{625B53C3-AB48-4EC1-BA1F-A1EF4146FC19}"},
                {"%USERPROFILE%\\Pictures", "{33E28130-4E1E-4676-835A-98395C3BC3BB}"},
                {"%LOCALAPPDATA%\\Microsoft\\Windows\\History", "{D9DC8A3B-B784-432E-A781-5A1130A75963}"},
                {"%USERPROFILE%\\Music\\Playlists", "{DE92C1C7-837F-4F69-A3BB-86E631204A23}"},
                {"%ALLUSERSPROFILE%\\Microsoft\\Windows\\GameExplorer", "{DEBF2536-E1A8-4c59-B6A2-414586476AEA}"},
                {"%ALLUSERSPROFILE%", "{62AB5D82-FDC1-4DC3-A9DD-070D1D495D97}"},
                {
                    "%LOCALAPPDATA%\\Microsoft\\Windows Photo Gallery\\Original Images",
                    "{2C36C0AA-5812-4b87-BFD0-4CD0DFB19B39}"
                },
                {
                    "%APPDATA%\\Microsoft\\Windows\\Libraries\\Videos.library-ms",
                    "{491E922F-5643-4AF4-A7EB-4E7A138D8174}"
                },
                {
                    "%USERPROFILE%\\AppData\\Roaming\\Microsoft\\Windows\\AccountPictures",
                    "{008ca0b1-55b4-4c56-b8a8-4de4b299d3be}"
                },
                {"%USERPROFILE%\\Pictures\\Screenshots", "{b7bede81-df94-4682-a7d8-57a52620b86f}"},
                {"%windir%\\system32", "{1AC14E77-02E7-4E5D-B744-2EB1AE5198B7}"},
                {"%APPDATA%\\Microsoft\\Windows\\Libraries\\Music.library-ms", "{2112AB0A-C86A-4FFE-A368-0DE96E47012E}"},
                {"%APPDATA%\\Microsoft\\Windows\\Templates", "{A63293E8-664E-48DB-A079-DF759E0509F7}"},
                {"%windir%\\Fonts", "{FD228CB7-AE11-4AE3-864C-16F3910AB8FE}"},
                {"%ProgramFiles%\\Windows Sidebar\\Gadgets", "{7B396E54-9EC5-4300-BE0A-2482EBAE1A26}"},
                {"%SystemDrive%\\Users", "{0762D272-C50A-4BB0-A382-697DCD729B80}"},
                {"%USERPROFILE%\\Saved Games", "{4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4}"},
                {
                    "%ALLUSERSPROFILE%\\Microsoft\\Windows\\Start Menu\\Programs",
                    "{0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8}"
                },
                {"%LOCALAPPDATA%\\Microsoft\\Windows\\Burn\\Burn", "{9E52AB10-F80D-49DF-ACB8-4330F5687855}"},
                {"%USERPROFILE%\\Contacts", "{56784854-C6CB-462b-8169-88E350ACB882}"},
                {"%PUBLIC%\\Videos\\Sample Videos", "{859EAD94-2E85-48AD-A71A-0969CB56A6CD}"},
                {"%USERPROFILE%\\Downloads", "{374DE290-123F-4565-9164-39C4925E467B}"},
                {"%SystemDrive%\\ProgramData)", "{62AB5D82-FDC1-4DC3-A9DD-070D1D495D97}"},
                {"%ALLUSERSPROFILE%\\OEM Links", "{C1BAE2D0-10DF-4334-BEDD-7AA20B227A9D}"},
                {"%PUBLIC%\\Music\\Sample Playlists", "{15CA69B3-30EE-49C1-ACE1-6B5EC372AFB5}"},
                {
                    "%ALLUSERSPROFILE%\\Microsoft\\Windows\\Start Menu\\Programs\\Administrative Tools",
                    "{D0384E7D-BAC3-4797-8F14-CBA229B392B5}"
                },
                {"%USERPROFILE%\\AppData\\Roaming", "{3EB685DB-65F9-4CF6-A03A-E3EF65729F3D}"},
                {"%APPDATA%", "{3EB685DB-65F9-4CF6-A03A-E3EF65729F3D}"},
                {"%ProgramFiles%", "{6D809377-6AF0-444b-8957-A3773F02200E}"},
                {
                    "%APPDATA%\\Microsoft\\Internet Explorer\\Quick Launch\\User Pinned\\ImplicitAppShortcuts",
                    "{BCB5256F-79F6-4CEE-B725-DC34E402FD46}"
                },
                {"%PUBLIC%\\Music\\Sample Music", "{B250C668-F57D-4EE1-A63C-290EE7D1AA1F}"},
                {"%ALLUSERSPROFILE%\\Microsoft\\Windows\\Start Menu", "{A4115719-D62E-491D-AA7C-E74B8BE3B067}"},
                {"%ALLUSERSPROFILE%\\Microsoft\\Windows\\Ringtones", "{E555AB60-153B-4D17-9F04-A5FE99FC15EC}"},
                {"%APPDATA%\\Microsoft\\Windows\\Network Shortcuts", "{C5ABBF53-E17F-4121-8900-86626FC2C973}"},
                {"%ProgramData%", "{62AB5D82-FDC1-4DC3-A9DD-070D1D495D97}"},
                {"%ProgramFiles%\\Common Files", "{6365D5A7-0F0D-45E5-87F6-0DA56B6A4F7D}"},
                {
                    "%USERPROFILE%\\AppData\\Local\\Microsoft\\Windows\\RoamingTiles",
                    "{00BCFC5A-ED94-4e48-96A1-3F6217F21990}"
                },
                {"%USERPROFILE%", "{5E6C858F-0E22-4760-9AFE-EA3317B67173}"},
                {"%LOCALAPPDATA%\\Microsoft\\Windows\\Ringtones", "{C870044B-F49E-4126-A9C3-B52A1FF411E8}"},
                {"%PUBLIC%\\Documents", "{ED4824AF-DCE4-45A8-81E2-FC7965083634}"},
                {"%PUBLIC%\\AccountPictures", "{0482af6c-08f1-4c34-8c90-e17ec98b1e17}"},
                {"%USERPROFILE%\\Music", "{4BD8D571-6D19-48D3-BE97-422220080E43}"},
                {
                    "%APPDATA%\\Microsoft\\Internet Explorer\\Quick Launch\\User",
                    "{9E3995AB-1F9C-4F13-B827-48B24B6C7174}"
                },
                {
                    "%USERPROFILE%\\AppData\\Local\\Microsoft\\Windows\\RoamedTileImages",
                    "{AAA8D5A5-F1D6-4259-BAA8-78E7EF60835E}"
                },
                {"%USERPROFILE%\\Pictures\\Slide Shows", "{69D2CF90-FC33-4FB7-9A0C-EBB0F0FCB43C}"},
                {
                    "%USERPROFILE%\\AppData\\Local\\Microsoft\\Windows Sidebar\\Gadgets",
                    "{A75D362E-50FC-4fb7-AC2C-A8BEAA314493}"
                },
                {"%ALLUSERSPROFILE%\\Microsoft\\Windows\\Templates", "{B94237E7-57AC-4347-9151-B08C6C32D1F7}"},
                {"%APPDATA%\\Microsoft\\Windows\\SendTo", "{8983036C-27C0-404B-8F08-102D10DCFD74}"},
                {"%PUBLIC%\\Music", "{3214FAB5-9757-4298-BB61-92A9DEAA44FF}"},
                {
                    "%APPDATA%\\Microsoft\\Windows\\Libraries\\Pictures.library-ms",
                    "{A990AE9F-A03B-4E80-94BC-9912D7504104}"
                },
                {
                    "%ALLUSERSPROFILE%\\Microsoft\\Windows\\Start Menu\\Programs\\StartUp",
                    "{82A5EA35-D9CD-47C5-9629-E15D2F714E6E}"
                }
            };

        static AppId()
        {
            for (var i = 0; i < 256; i++)
            {
                var lv = (ulong) i;
                for (var j = 0; j < 8; j++)
                {
                    ulong fl = lv & 1;
                    lv = lv >> 1;
                    if (fl == 1) lv = lv ^ CrcPolynomial;
                    CrcTable[i] = lv;
                }
            }
        }

        public AppId(string executable, string appId, bool custom)
        {
            Executable = executable;
            AppUserModelId = appId;
            Custom = custom;
        }

        public string Executable { get; set; }
        public string AppUserModelId { get; set; }
        public bool Custom { get; set; }
        public string DestinationList
        {
            get
            {
                if (Custom)
                {
                    var path = Environment.ExpandEnvironmentVariables(CustomDestinationsJumplistFolder);
                    var file = Path.Combine(path, AppUserModelId.ToLower()) + ".customDestinations-ms";
                    return File.Exists(file) ? file : null;
                }
                else
                {
                    var path = Environment.ExpandEnvironmentVariables(AutomaticDestinationsJumplistFolder);
                    var file = Path.Combine(path, AppUserModelId.ToLower()) + ".automaticDestinations-ms";
                    return File.Exists(file) ? file : null;
                }
            }
        }

        public static List<AppId> Find(IntPtr windowHandle)
        {
            var explicitId = Api.GetWindowAppId(windowHandle);
            var processId = (int)User32.GetWindowThreadProcessId(windowHandle);
            var process = Process.GetProcessById(processId);

            if (explicitId != null)
            {
                var data = CreateFormattedName(explicitId);
                var appId = CalculateCrc64(data);
                return new List<AppId> { new AppId(process.MainModule.FileName, appId, true) };
            }

            // No associated windows or those windows didn't define an explicit AppId
            // So, try to build an automatic AppId from the location and executable name
            var path = Path.GetDirectoryName(process.MainModule.FileName);
            path = TrySubstituteEnvironmentVariable(path);
            path = TryReplaceWithKnownFolder(path);
            var appIds = CalculateAppIds(true, Path.GetFileName(process.MainModule.FileName), FindMatchingDirectories(path));
            return appIds.Select(a => new AppId(process.MainModule.FileName, a, false)).ToList();
        }

        public static List<AppId> Find(string executable)
        {
            // First, check if there are any open windows matching this executable
            var friendlyName = Path.GetFileNameWithoutExtension(executable);
            var process = Process.GetProcessesByName(friendlyName).Single(p => p.MainWindowHandle != IntPtr.Zero);
            var explicitId = Api.GetWindowAppId(process.MainWindowHandle);

            if (explicitId != null)
            {
                var data = CreateFormattedName(explicitId);
                var appId = CalculateCrc64(data);
                return new List<AppId> { new AppId("", appId, true) };
            }

            // No associated windows or those windows didn't define an explicit AppId
            // So, try to build an automatic AppId from the location and executable name
            var path = Path.GetDirectoryName(executable);
            path = TrySubstituteEnvironmentVariable(path);
            path = TryReplaceWithKnownFolder(path);
            var appIds = CalculateAppIds(true, Path.GetFileName(executable), FindMatchingDirectories(path));
            return appIds.Select(a => new AppId(executable, a, false)).ToList();
        }

        private static IEnumerable<string> FindMatchingDirectories(string directory)
        {
            var found = new List<string>();
            string[] split = directory.ToLower().Split(Path.DirectorySeparatorChar);

            if (split[0] == "%windir%" && (split[1] == "system32" || split[1] == "syswow64" || split[1] == "sysnative"))
            {
                found.Add(SystemDependentKnownFolderIds["FOLDERID_System"]);
                found.Add(SystemDependentKnownFolderIds["FOLDERID_SystemX86"]);
            }
            else if (split[0] == "%programfiles%" && split[1] == "common files")
            {
                found.Add(SystemDependentKnownFolderIds["FOLDERID_ProgramFilesCommon"]);
                found.Add(SystemDependentKnownFolderIds["FOLDERID_ProgramFilesCommonX64"]);
                found.Add(SystemDependentKnownFolderIds["FOLDERID_ProgramFilesCommonX86"]);
            }
            else if (split[0] == "%programfiles")
            {
                var remaining = Path.Combine(new ArraySegment<string>(split, 1, split.Length - 1).Array);
                found.Add(Path.Combine(SystemDependentKnownFolderIds["FOLDERID_ProgramFilesCommon"], remaining));
                found.Add(Path.Combine(SystemDependentKnownFolderIds["FOLDERID_ProgramFilesCommonX64"], remaining));
                found.Add(Path.Combine(SystemDependentKnownFolderIds["FOLDERID_ProgramFilesCommonX86"], remaining));
            }
            else
            {
                found.Add(directory);
            }

            return found;
        }

        private static string TryReplaceWithKnownFolder(string path)
        {
            if (CommonKnownFolderIds.Keys.Contains(path, StringComparer.OrdinalIgnoreCase))
            {
                path = CommonKnownFolderIds[CommonKnownFolderIds.Keys.Single(i => i.Equals(path, StringComparison.OrdinalIgnoreCase))];
            }
            return path;
        }

        private static IEnumerable<string> CalculateAppIds(bool automatic, string executable, IEnumerable<string> directories)
        {
            var matchingIds = new List<string>();

            if (automatic)
            {
                var formattedNames = directories.Select(directory => CreateFormattedName(Path.Combine(directory, executable)));
                var processed = formattedNames.Select(CalculateCrc64);
                matchingIds.AddRange(processed);
            }
            else
            {
                var data = CreateFormattedName(executable);
                var appId = CalculateCrc64(data);
                matchingIds.Add(appId);
            }

            return matchingIds;
        }

        private static IEnumerable<byte> CreateFormattedName(string item)
        {
            var data = new List<byte>();
            foreach (char letter in item.ToUpper())
            {
                data.Add((byte) letter);
                data.Add(0);
            }
            return data;
        }

        private static string CalculateCrc64(IEnumerable<byte> data)
        {
            var crc = data.Aggregate(0xFFFFFFFFFFFFFFFF, (current, b) => (current >> 8) ^ CrcTable[(current ^ b) & 0xFF]);
            return (crc >> 32).ToString("X8") + (crc & 0xFFFFFFFF).ToString("X8");
        }

        private static string TrySubstituteEnvironmentVariable(string path)
        {
            IDictionary envrionmentVariables = Environment.GetEnvironmentVariables();
            var replacments = new[]
                {
                    "windir",
                    "appdata",
                    "localappdata",
                    "userprofile",
                    "public",
                    "allusersprofile",
                    "programdata",
                    "programfiles",
                    "systemdrive"
                };
            foreach (var repl in replacments)
            {
                var matchingKey = "";
                foreach (string key in envrionmentVariables.Keys)
                {
                    if (repl.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        matchingKey = key;
                        break;
                    }
                }
                if (matchingKey != "")
                {
                    var correspondingPath = (string) envrionmentVariables[matchingKey];
                    if (path.ToUpper().StartsWith(correspondingPath.ToUpper()))
                    {
                        return "%" + repl + "%" + path.Substring(correspondingPath.Length);
                    }
                }
            }
            return path;
        }

        private static class Api
        {
            private static string GetWindowProperty(IntPtr hwnd, PropertyKey propkey)
            {
                // Get the IPropertyStore for the given window handle
                var propStore = GetWindowPropertyStore(hwnd);
                string currentValue;

                using (var pv = new PropVariant())
                {
                    var r = propStore.GetValue(ref propkey, pv);
                    if (r != HResult.Ok)
                        throw Marshal.GetExceptionForHR((int) r);
                    currentValue = (string)pv.Value;
                }

                // Dispose the IPropertyStore and PropVariant
                Marshal.ReleaseComObject(propStore);
                return currentValue;
            }

            private static IPropertyStore GetWindowPropertyStore(IntPtr hwnd)
            {
                IPropertyStore propStore;
                var guid = new Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99");
                var rc = SHGetPropertyStoreForWindow(hwnd, ref guid, out propStore);
                if (rc != 0)
                    throw Marshal.GetExceptionForHR(rc);
                return propStore;
            }

            /// <summary>
            /// HRESULT Wrapper    
            /// </summary>    
            public enum HResult
            {
                /// <summary>     
                /// S_OK          
                /// </summary>    
                Ok = 0x0000,
            }

            /// <summary>
            /// Defines a unique key for a Shell Property
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 4)]
            public struct PropertyKey : IEquatable<PropertyKey>
            {
                private readonly Guid formatId;
                private readonly Int32 propertyId;

                /// <summary>
                /// PropertyKey Constructor
                /// </summary>
                /// <param name="formatId">A unique GUID for the property</param>
                /// <param name="propertyId">Property identifier (PID)</param>
                public PropertyKey(Guid formatId, Int32 propertyId)
                {
                    this.formatId = formatId;
                    this.propertyId = propertyId;
                }

                #region IEquatable<PropertyKey> Members

                /// <summary>
                /// Returns whether this object is equal to another. This is vital for performance of value types.
                /// </summary>
                /// <param name="other">The object to compare against.</param>
                /// <returns>Equality result.</returns>
                public bool Equals(PropertyKey other)
                {
                    return other.Equals((object)this);
                }

                #endregion

                #region equality and hashing

                /// <summary>
                /// Returns the hash code of the object. This is vital for performance of value types.
                /// </summary>
                /// <returns></returns>
                public override int GetHashCode()
                {
                    return formatId.GetHashCode() ^ propertyId;
                }

                /// <summary>
                /// Returns whether this object is equal to another. This is vital for performance of value types.
                /// </summary>
                /// <param name="obj">The object to compare against.</param>
                /// <returns>Equality result.</returns>
                public override bool Equals(object obj)
                {
                    if (obj == null)
                        return false;

                    if (!(obj is PropertyKey))
                        return false;

                    var other = (PropertyKey)obj;
                    return other.formatId.Equals(formatId) && (other.propertyId == propertyId);
                }

                /// <summary>
                /// Implements the == (equality) operator.
                /// </summary>
                /// <param name="propKey1">First property key to compare.</param>
                /// <param name="propKey2">Second property key to compare.</param>
                /// <returns>true if object a equals object b. false otherwise.</returns>        
                public static bool operator ==(PropertyKey propKey1, PropertyKey propKey2)
                {
                    return propKey1.Equals(propKey2);
                }

                /// <summary>
                /// Implements the != (inequality) operator.
                /// </summary>
                /// <param name="propKey1">First property key to compare</param>
                /// <param name="propKey2">Second property key to compare.</param>
                /// <returns>true if object a does not equal object b. false otherwise.</returns>
                public static bool operator !=(PropertyKey propKey1, PropertyKey propKey2)
                {
                    return !propKey1.Equals(propKey2);
                }

                #endregion
            }

            /// <summary>
            /// A property store
            /// </summary>
            [ComImport]
            [Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IPropertyStore
            {
                /// <summary>
                /// Gets the number of properties contained in the property store.
                /// </summary>
                /// <param name="propertyCount"></param>
                /// <returns></returns>
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                HResult GetCount([Out] out uint propertyCount);

                /// <summary>
                /// Get a property key located at a specific index.
                /// </summary>
                /// <param name="propertyIndex"></param>
                /// <param name="key"></param>
                /// <returns></returns>
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                HResult GetAt([In] uint propertyIndex, out PropertyKey key);

                /// <summary>
                /// Gets the value of a property from the store
                /// </summary>
                /// <param name="key"></param>
                /// <param name="pv"></param>
                /// <returns></returns>
                //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                HResult GetValue([In] ref PropertyKey key, [Out] PropVariant pv);

                /// <summary>
                /// Sets the value of a property in the store
                /// </summary>
                /// <param name="key"></param>
                /// <param name="pv"></param>
                /// <returns></returns>
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
                HResult SetValue([In] ref PropertyKey key, [In] PropVariant pv);

                /// <summary>
                /// Commits the changes.
                /// </summary>
                /// <returns></returns>
                [PreserveSig]
                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
                HResult Commit();
            }

            internal static string GetWindowAppId(IntPtr hwnd)
            {
                var key = new PropertyKey(new Guid("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}"), 5);
                return GetWindowProperty(hwnd, key);
            }

            [DllImport("shell32.dll")]
            private static extern int SHGetPropertyStoreForWindow(
                IntPtr hwnd,
                ref Guid iid /*IID_IPropertyStore*/,
                [Out, MarshalAs(UnmanagedType.Interface)]out IPropertyStore propertyStore);

            [DllImport("Ole32.dll", PreserveSig = false)] // returns hresult
            private extern static void PropVariantClear([In, Out] PropVariant pvar);

            /// <summary>
            /// Represents the OLE struct PROPVARIANT.
            /// This class is intended for internal use only.
            /// </summary>
            /// <remarks>
            /// Originally sourced from http://blogs.msdn.com/adamroot/pages/interop-with-propvariants-in-net.aspx
            /// and modified to support additional types including vectors and ability to set values
            /// </remarks>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "_ptr2")]
            [StructLayout(LayoutKind.Explicit)]
            public sealed class PropVariant : IDisposable
            {
                #region Fields
#pragma warning disable 649
                [FieldOffset(0)]
                decimal _decimal;

                // This is actually a VarEnum value, but the VarEnum type
                // requires 4 bytes instead of the expected 2.
                [FieldOffset(0)]
                ushort _valueType;

                // Reserved Fields
                //[FieldOffset(2)]
                //ushort _wReserved1;
                //[FieldOffset(4)]
                //ushort _wReserved2;
                //[FieldOffset(6)]
                //ushort _wReserved3;

                // In order to allow x64 compat, we need to allow for
                // expansion of the IntPtr. However, the BLOB struct
                // uses a 4-byte int, followed by an IntPtr, so
                // although the valueData field catches most pointer values,
                // we need an additional 4-bytes to get the BLOB
                // pointer. The valueDataExt field provides this, as well as
                // the last 4-bytes of an 8-byte value on 32-bit
                // architectures.
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
                [FieldOffset(12)]
                IntPtr _ptr2;
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
                [FieldOffset(8)]
                IntPtr _ptr;
                [FieldOffset(8)]
                Int32 _int32;
                [FieldOffset(8)]
                UInt32 _uint32;
                [FieldOffset(8)]
                byte _byte;
                [FieldOffset(8)]
                sbyte _sbyte;
                [FieldOffset(8)]
                short _short;
                [FieldOffset(8)]
                ushort _ushort;
                [FieldOffset(8)]
                long _long;
                [FieldOffset(8)]
                ulong _ulong;
                [FieldOffset(8)]
                double _double;
                [FieldOffset(8)]
                float _float;
#pragma warning restore 649

                #endregion // struct fields

                #region public Properties

                /// <summary>
                /// Gets or sets the variant type.
                /// </summary>
                private VarEnum VarType
                {
                    get { return (VarEnum)_valueType; }
                }

                /// <summary>
                /// Gets the variant value.
                /// </summary>
                public object Value
                {
                    get
                    {
                        switch ((VarEnum)_valueType)
                        {
                            case VarEnum.VT_I1:
                                return _sbyte;
                            case VarEnum.VT_UI1:
                                return _byte;
                            case VarEnum.VT_I2:
                                return _short;
                            case VarEnum.VT_UI2:
                                return _ushort;
                            case VarEnum.VT_I4:
                            case VarEnum.VT_INT:
                                return _int32;
                            case VarEnum.VT_UI4:
                            case VarEnum.VT_UINT:
                                return _uint32;
                            case VarEnum.VT_I8:
                                return _long;
                            case VarEnum.VT_UI8:
                                return _ulong;
                            case VarEnum.VT_R4:
                                return _float;
                            case VarEnum.VT_R8:
                                return _double;
                            case VarEnum.VT_BOOL:
                                return _int32 == -1;
                            case VarEnum.VT_ERROR:
                                return _long;
                            case VarEnum.VT_CY:
                                return _decimal;
                            case VarEnum.VT_DATE:
                                return DateTime.FromOADate(_double);
                            case VarEnum.VT_FILETIME:
                                return DateTime.FromFileTime(_long);
                            case VarEnum.VT_BSTR:
                                return Marshal.PtrToStringBSTR(_ptr);
                            case VarEnum.VT_BLOB:
                                return GetBlobData();
                            case VarEnum.VT_LPSTR:
                                return Marshal.PtrToStringAnsi(_ptr);
                            case VarEnum.VT_LPWSTR:
                                return Marshal.PtrToStringUni(_ptr);
                            case VarEnum.VT_UNKNOWN:
                                return Marshal.GetObjectForIUnknown(_ptr);
                            case VarEnum.VT_DISPATCH:
                                return Marshal.GetObjectForIUnknown(_ptr);
                            case VarEnum.VT_DECIMAL:
                                return _decimal;
                            default:
                                // if the value cannot be marshaled
                                return null;
                        }
                    }
                }

                #endregion

                #region Private Methods

                private object GetBlobData()
                {
                    var blobData = new byte[_int32];

                    IntPtr pBlobData = _ptr2;
                    Marshal.Copy(pBlobData, blobData, 0, _int32);

                    return blobData;
                }

                #endregion

                #region IDisposable Members

                /// <summary>
                /// Disposes the object, calls the clear function.
                /// </summary>
                public void Dispose()
                {
                    PropVariantClear(this);

                    GC.SuppressFinalize(this);
                }

                /// <summary>
                /// Finalizer
                /// </summary>
                ~PropVariant()
                {
                    Dispose();
                }

                #endregion

                /// <summary>
                /// Provides an simple string representation of the contained data and type.
                /// </summary>
                /// <returns></returns>
                public override string ToString()
                {
                    return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "{0}: {1}", Value, VarType.ToString());
                }

            }
        }
    }
}