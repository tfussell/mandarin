using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace WinDock3.Business.Persistence
{
    public abstract class JsonSerializer<T> : Serializer<T> where T : new()
    {
        private readonly JavaScriptSerializer serializer;

        protected JsonSerializer(string file) : base(file)
        {
            serializer = new JavaScriptSerializer();
        }

        protected Dictionary<string, object> ReadJsonObject()
        {
            var json = global::System.IO.File.ReadAllText(File);
            return serializer.Deserialize<Dictionary<string, object>>(json);
        }

        protected void WriteJsonObject(Dictionary<string, object> jsonObject)
        {
            var json = serializer.Serialize(jsonObject);
            global::System.IO.File.WriteAllText(File, json);
        }
    }
}
