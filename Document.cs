namespace Deserializer
{
    using System.Collections.Generic;

    public class Document
    {
        public Document()
        {
            this.Entries = new Dictionary<string, Entry>();
        }

        public bool? HasTitle { get; set; }

        public string Title { get; set; }

        public Dictionary<string, Entry> Entries { get; set; }
    }
}