namespace Deserializer
{
    using System;

    //// TODO will each entry be of the same shape? are any of those fields optional?
    public class Entry
    {
        public bool ThumbExists { get; set; }

        public Uri Path { get; set; }
        
        public DateTimeOffset ClientMtime { get; set; }

        public long Bytes { get; set; }
    }
}