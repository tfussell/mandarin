using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WinDock.Configuration;

namespace WinDock.GUI
{
    public class TabbedPanel : UserControl
    {
        public delegate void ProfileChangedDelegate(Profile active);

        public event ProfileChangedDelegate ProfileChanged = delegate { };
 
        private Panel holder;
        private List<Profile> profiles;

        public TabbedPanel(List<Profile> profiles)
        {
            this.profiles = profiles;
            Initialize();
        }

        private void Initialize()
        {
            AutoSize = true;

            var panel = new TableLayoutPanel
            {
                AutoSize = true,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                ColumnCount = 2,
                RowCount = 1
            };

            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var bl = new ButtonList("Options");
            bl.SelectionChanged += ChangePanel;

            bl.AddButton("General");
            bl.AddButton("Docks");
            bl.AddButton("Themes");
            bl.AddButton("Plugins");
            bl.AddButton("Services");

            Margin = Padding.Empty;
            Padding = Padding.Empty;

            panel.Controls.Add(bl, 0, 0);

            holder = new Panel { Padding = Padding.Empty, Margin = Padding.Empty, Size = new Size(500, 400) };
            panel.Controls.Add(holder, 1, 0);

            Controls.Add(panel);

            bl.Selected = "General";
        }

        private void ChangePanel(string item)
        {
            if (holder.Controls.Count > 0)
                holder.Controls.Clear();

            if (item == "General")
            {
                var gop = new GeneralOptionsPanel(profiles);
                gop.ProfileChanged += active => ProfileChanged(active);
                holder.Controls.Add(gop);
            }
            else
            {
                var l = new Label {Text = item};
                holder.Controls.Add(l);
            }
        }
    }
}