using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinDock.Themes
{
    public class DockItemStyle : BoundableTheme
    {
        public static DockItemStyle Default
        {
            get
            {
                return new DockItemStyle();
            }
        }

        public DockItemStyle(string themeFile) : base(themeFile)
        {
            
        }

        private DockItemStyle()
        {
            
        }

        protected override void SetDefault()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.Transparent;
            BackgroundAlpha = 0.0F;
            BackgroundImage = null;
            BackgroundImageLayout = ImageLayout.None;
            BorderAlpha = 0.0F;
            BorderColor = Color.Transparent;
            BorderImageLayouts = new[] { ImageLayout.None, ImageLayout.None, ImageLayout.None, ImageLayout.None };
            BorderImages = new string[] {null, null, null, null};
            BorderStyle = BoundableBorderStyle.None;
            BorderWidth = 0.0F;
            CornerImageLayouts = new[] { ImageLayout.None, ImageLayout.None, ImageLayout.None, ImageLayout.None };
            CornerImages = new string[] {null, null, null, null};
            Margin = Padding.Empty;
            MaximumSize = Size.Empty;
            MinimumSize = Size.Empty;
            Padding = Padding.Empty;
            RotateBackgroundWithEdge = false;
            RotateBorderWithEdge = true;
            RotateContentsWithEdge = false;
            Size = Size.Empty;
        }
    }
}
