namespace Deserializer
{
    using System;
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

        public class Entry
        {
            public bool? ThumbExists { get; set; }

            public Uri Path { get; set; }

            public DateTimeOffset? ClientMtime { get; set; }

            public long? Bytes { get; set; }
        }

    }
}