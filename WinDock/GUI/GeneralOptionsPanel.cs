using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinDock.Configuration;
using System.Linq;

namespace WinDock.GUI
{
    public class GeneralOptionsPanel : Panel
    {
        public delegate void ProfileChangedDelegate(Profile active);

        public event ProfileChangedDelegate ProfileChanged = delegate { };

        public GeneralOptionsPanel(List<Profile> profiles)
        {
            Width = 500;
            Height = 400;
            var p = new TableLayoutPanel
                {
                    Padding = new Padding {All = 10},
                    AutoSize = true,
                    RowCount = 10,
                    ColumnCount = 2
                };
            p.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            p.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            p.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            p.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            p.Controls.Add(new NiceLabel("General"), 0, 0);
            var autorunLabel = new NiceLabel("Autorun") {Anchor = AnchorStyles.Right};
            p.Controls.Add(autorunLabel, 0, 1);
            p.Controls.Add(new CheckBox(), 1, 1);
            var cbLabel = new NiceLabel("Active Profile") { Anchor = AnchorStyles.Right };
            p.Controls.Add(cbLabel, 0, 2);
            var cb = new ComboBox();
            foreach (var s in profiles.Select(i => Path.GetDirectoryName(i.Name).Split('\\').Last()))
            {
                cb.Items.Add(s);
            }
            cb.SelectedIndexChanged += (sender, args) => ProfileChanged(profiles[cb.SelectedIndex]);
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            //cb.AutoCompleteMode = AutoCompleteMode.Suggest;
            p.Controls.Add(cb, 1, 2);
            Controls.Add(p);
        }

        public void Initialize()
        {
            AutoSize = true;
            BackColor = Color.FromArgb(236, 240, 241);
        }
    }
}