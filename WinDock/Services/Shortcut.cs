using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Shell32;
using WindowsShellFacade;

namespace WinDock.Services
{
    class Shortcut
    {
        public string Filename { get; set; }
        public string Target { get; set; }

        public Shortcut(string s)
        {
            Filename = s;
            Target = ParseShortcut(s);
        }

        public static string ParseShortcut(String lnkfile)
        {
            return LocateReferentNormal(lnkfile) ?? LocateReferentMsi(lnkfile);
        }

        private static string LocateReferentNormal(string lnkFile)
        {
            var shl = new Shell();
            var dir = shl.NameSpace(Path.GetDirectoryName(lnkFile));
            var itm = dir.Items().Item(Path.GetFileName(lnkFile));
            var lnk = itm.GetLink;

            if(lnk != null)
                lnk.Resolve(8);

            return lnk != null ? lnk.Path : null;
        }

        private static string LocateReferentMsi(string lnkFile)
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
    }
}
