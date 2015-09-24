namespace Deserializer
{
    using System.Collections.Generic;

    public class Document
    {
        //// TODO does it relate to Title somehow?
        public bool HasTitle { get; set; }

        public string Title { get; set; }

        public Dictionary<string, Entry> Entries
        {
            get;
            set;
        }
    }
}