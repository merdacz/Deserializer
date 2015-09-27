namespace Deserializer
{
    using System;

    public class Entry
    {
        public bool? ThumbExists { get; set; }

        public Uri Path { get; set; }

        public DateTimeOffset? ClientMtime { get; set; }

        public long? Bytes { get; set; }
    }
}