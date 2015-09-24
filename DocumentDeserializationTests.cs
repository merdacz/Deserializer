namespace Deserializer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using FluentAssertions;

    using RestSharp;
    using RestSharp.Deserializers;

    using Xunit;

    public class DocumentDeserializationTests
    {
        private static string DateFormat = "ddd, dd MMM yyyy HH:mm:sszzz";

        [Fact]
        public void AlternativeStructureDeserialization()
        {
            var response = new RestResponse();
            response.Content = Inputs.Alternative;
            response.ContentType = "text/json";

            var sut = CreateDeserializer();
            var result = sut.Deserialize<Document>(response);

            result.ShouldBeEquivalentTo(Expected);
        }

        [Fact(Skip = "Would require List<dynamic> for Entries to pass. ")]
        public void OriginalStructureDeserialization()
        {
            var response = new RestResponse();
            response.Content = Inputs.Original;
            response.ContentType = "text/json";

            var sut = CreateDeserializer();
            var result = sut.Deserialize<Document>(response);

            result.ShouldBeEquivalentTo(Expected);
        }

        private static Document Expected 
        {
            get
            {
                var gettingStarted = new Entry()
                                         {
                                             ThumbExists = false,
                                             Path = new Uri("/GettingStarted.pdf", UriKind.Relative),
                                             ClientMtime =
                                                 new DateTimeOffset(2014, 1, 8, 18, 0, 54, TimeSpan.Zero),
                                             Bytes = 249159
                                         };
                var task = new Entry()
                               {
                                   ThumbExists = true,
                                   Path = new Uri("/Task.jpg", UriKind.Relative),
                                   ClientMtime = new DateTimeOffset(2014, 1, 14, 5, 53, 57, TimeSpan.Zero),
                                   Bytes = 207696
                };
                return new Document()
                           {
                               Title = "GoodLuck",
                               HasTitle = true,
                               Entries =
                                   new Dictionary<string, Entry>()
                                       {
                                           ["/gettingstarted.pdf"] = gettingStarted,
                                           ["/task.jpg"] = task
                                       }
                           };
            }
        }

        private static JsonDeserializer CreateDeserializer()
        {
            var deserializer = new JsonDeserializer();
            deserializer.Culture = CultureInfo.InvariantCulture;
            deserializer.DateFormat = DateFormat;
            return deserializer;
        }

        [Fact]
        public void DateFormatTest()
        {
            var format = DateFormat;
            var now = new DateTimeOffset(2015, 9, 24, 21, 16, 37, TimeSpan.FromHours(2));
            var actual = now.ToString(format, CultureInfo.InvariantCulture);
            Assert.Equal("Thu, 24 Sep 2015 21:16:37+02:00", actual);
        }
    }
}

