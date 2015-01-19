using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mandarin.Business.Themes;

namespace Mandarin.Business.Persistence
{
    public class ThemeSerializer : JsonSerializer<Theme>
    {
        public ThemeSerializer(string file) : base(file)
        {
        }

        public override Theme Deserialize()
        {
            var jsonObject = ReadJsonObject();
            var values = (Dictionary<string, object>)jsonObject;
            var theme = new Theme();
            var deserializationMethods = GetType().GetMethods(BindingFlags.Public).ToList();
            foreach (var property in typeof(DockItemStyle).GetProperties())
            {
                var deserializationMethod = deserializationMethods.Single(m => m.Name == "Deserialize" + property.PropertyType);
                var value = deserializationMethod.Invoke(null, new[] { values[property.Name] });
                property.SetValue(theme, value, null);
            }
            return theme;
        }

        private DockItemStyle DeserializeDockItemStyle(object serialized)
        {
            var values = (Dictionary<string, object>) serialized;
            var itemStyle = new DockItemStyle("");
            var deserializationMethods = GetType().GetMethods(BindingFlags.Public).ToList();
            foreach (var property in typeof(DockItemStyle).GetProperties())
            {
                var deserializationMethod = deserializationMethods.Single(m => m.Name == "Deserialize" + property.PropertyType);
                var value = deserializationMethod.Invoke(null, new[] {values[property.Name]});
                property.SetValue(itemStyle, value, null);
            }
            return itemStyle;
        }

        public override void Serialize(Theme serializable)
        {
            var jsonObject = new Dictionary<string, object>
                {
                    //{"DockItemStyle", SerializeDockItemStyle(serializable.DockItemStyle)},
                    //{"DockStyle", SerializeDockStyle(serializable.DockStyle)}
                };
            WriteJsonObject(jsonObject);
        }


    }
}
