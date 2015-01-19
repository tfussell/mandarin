namespace Mandarin.Business.Persistence
{
    public abstract class Serializer<T> where T : new()
    {
        public string File
        {
            get { return file; }
            set { file = value; }
        }

        private string file;

        protected Serializer(string file)
        {
            this.file = file;
        }

        public abstract T Deserialize();
        public abstract void Serialize(T serializable);
    }
}
