using System.Drawing;

namespace WinDock.Business.Themes
{
    public class DockStyle : BoundableTheme
    {
        public DockItemStyle DockItemStyle { get; set; }

        public DockStyle(string themeFile) : base(themeFile)
        {
        }

        protected override void SetDefault()
        {
            AutoSize = true;
            BackColor = Color.Black;
            BackgroundAlpha = 1.0F;
            BackgroundImage = null;
            BackgroundImageLayout = ImageLayout.None;
            BorderAlpha = 1.0F;
            BorderColor = Color.WhiteSmoke;
            BorderImageLayouts = new[] {ImageLayout.None, ImageLayout.None, ImageLayout.None, ImageLayout.None};
            BorderImages = new string[] {null, null, null, null};
            BorderStyle = BoundableBorderStyle.RoundedLineThree;
            BorderWidth = 3.0F;
            CornerImageLayouts = new[] {ImageLayout.None, ImageLayout.None, ImageLayout.None, ImageLayout.None};
            CornerImages = new string[] {null, null, null, null};
            DockItemStyle = DockItemStyle.Default;
            Margin = Padding.Zero;
            MaximumSize = Size.Empty;
            MinimumSize = Size.Empty;
            Padding = Padding.Zero;
            RotateBackgroundWithEdge = false;
            RotateBorderWithEdge = true;
            RotateContentsWithEdge = false;
            Size = Size.Empty;
        }
    }
}