using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace WinDock3.Business.Themes
{
    public class Padding
    {
        public Padding(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Padding Zero
        {
            get { return new Padding(0, 0, 0, 0); }
        }

        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public enum ImageLayout
    {
        None,
        Center,
        Stretch,
        Tile,
        Zoom
    }

    public enum BoundableBorderStyle
    {
// ReSharper disable UnusedMember.Global
        None,
        ImageThree,
        ImageFour,
        LineThree,
        LineFour,
        RoundedLineThree,
        RoundedLineFour
// ReSharper restore UnusedMember.Global
    }

    [Serializable]
    public abstract class BoundableTheme
    {
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public bool RotateContentsWithEdge { get; set; }
        public Padding Padding { get; set; }
        public Padding Margin { get; set; }
        public Size MinimumSize { get; set; }
        public Size MaximumSize { get; set; }
        public Size Size { get; set; }
        public bool AutoSize { get; set; }
        public string[] BorderImages { get; set; }
        public ImageLayout[] BorderImageLayouts { get; set; }
        public string[] CornerImages { get; set; }
        public ImageLayout[] CornerImageLayouts { get; set; }
        public Color BackColor { get; set; }
        public float BackgroundAlpha { get; set; }
        public string BackgroundImage { get; set; }
        public ImageLayout BackgroundImageLayout { get; set; }
        public bool RotateBackgroundWithEdge { get; set; }
        public Color BorderColor { get; set; }
        public float BorderAlpha { get; set; }
        public BoundableBorderStyle BorderStyle { get; set; }
        public float BorderWidth { get; set; }
        public bool RotateBorderWithEdge { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global
// ReSharper restore MemberCanBeProtected.Global

        private string ThemeFile { get; set; }

        protected BoundableTheme(string themeFile)
        {
            ThemeFile = themeFile;
            LoadOrSetDefault();
        }

        protected BoundableTheme()
        {
            LoadOrSetDefault();
        }

        ~BoundableTheme()
        {
            Save();
        }

        private void LoadOrSetDefault()
        {
            if (File.Exists(ThemeFile))
            {
                Load();
            }
            else
            {
                SetDefault();
            }
        }

        private void Save()
        {
            using (var fileStream = File.Open(ThemeFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var writer = new StreamWriter(fileStream);
                foreach (var field in GetType().GetFields(BindingFlags.Public))
                {
                    var value = field.GetValue(this);
                    var type = field.FieldType;
                    var serialized = SerializeMember(value, type);
                    writer.WriteLine("{0} {1}", field.Name, serialized);
                }
            }
        }

        private void Load()
        {
            using (var fileStream = File.Open(ThemeFile, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(fileStream);
                foreach (var field in GetType().GetFields(BindingFlags.Public))
                {
                    var line = reader.ReadLine();
                    var type = field.FieldType;
                    var deserialized = DeserializeMember(line, type);
                    field.SetValue(this, deserialized);
                }
            }
        }

        private object DeserializeMember(string serialized, Type type)
        {
            if (type == typeof (bool))
            {
                if (serialized == "true")
                {
                    return true;
                }
                if (serialized == "false")
                {
                    return false;
                }
            }
            throw new Exception("Invalid value");
        }

        private static string SerializeMember(object member, Type type)
        {
            if (type == typeof(bool))
            {
                var value = (bool)member;
                return value ? "true" : "false";
            }
            if (type == typeof(Padding))
            {
                var value = (Padding)member;
                return String.Format("Padding {0} {1} {2} {3}", value.Left, value.Top, value.Right, value.Bottom);
            }
            if (type == typeof(Size))
            {
                var value = (Size)member;
                return String.Format("Size {0} {1}", value.Width, value.Height);
            }
            if (type == typeof(string[]))
            {
                var value = (string[])member;
                return string.Join(" ", value);
            }

            throw new Exception("Unknown type: " + type);
        }

        protected abstract void SetDefault();
    }
}