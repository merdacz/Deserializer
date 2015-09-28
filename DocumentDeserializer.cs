namespace Deserializer
{
    using System.Collections.Generic;
    using System.Globalization;

    using RestSharp;
    using RestSharp.Deserializers;
    using RestSharp.Serializers;

    /// <summary>
    /// Document deserializer 
    /// </summary>
    /// <remarks>Uses RestSharp underneath. Two stage approach due to RestSharp deserializer
    /// limitations around highly polymorphic JSON array instances. </remarks>
    /// <see cref="http://stackoverflow.com/a/29217883"/>
    public class DocumentDeserializer
    {
        public static string DateFormat = "ddd, dd MMM yyyy HH:mm:sszzz";

        public Document Deserialize(string content)
        {
            var response = this.WrapWithResponse(content);
            var deserializer = CreateDeserializer();
            var interim = deserializer.Deserialize<InterimDocumentRepresentation>(response);
            return this.MapToFullyTypedDocument(interim, deserializer);
        }

        private Document MapToFullyTypedDocument(InterimDocumentRepresentation interim, JsonDeserializer deserializer)
        {
            //// TODO add guarding checks to make sure assumptions made will hold. 
            var result = new Document();
            result.HasTitle = interim.HasTitle;
            result.Title = interim.Title;
            foreach (var entry in interim.Entries)
            {
                var entrySource = this.SerializeRaw(entry[1]);
                var entryWrapped = this.WrapWithResponse(entrySource);
                var typedEntry = deserializer.Deserialize<Document.Entry>(entryWrapped);
                result.Entries.Add(entry[0].ToString(), typedEntry);
            }

            return result;
        }

        private RestResponse WrapWithResponse(string content)
        {
            var response = new RestResponse();
            response.Content = content;
            response.ContentType = "text/json";
            return response;
        }

        private static JsonDeserializer CreateDeserializer()
        {
            var deserializer = new JsonDeserializer();
            deserializer.Culture = CultureInfo.InvariantCulture;
            deserializer.DateFormat = DateFormat;
            return deserializer;
        }

        private string SerializeRaw(object @object)
        {
            var serializer = new JsonSerializer();
            serializer.DateFormat = DateFormat;
            return serializer.Serialize(@object);
        }

        private class InterimDocumentRepresentation
        {
            public bool? HasTitle { get; set; }

            public string Title { get; set; }

            public List<JsonArray> Entries { get; set; }
        }
    }
}