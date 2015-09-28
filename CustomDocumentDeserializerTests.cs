namespace Deserializer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    using FluentAssertions;

    using RestSharp;
    using RestSharp.Deserializers;
    using RestSharp.Serializers;

    using Xunit;
    using Xunit.Abstractions;

    public class CustomDocumentDeserializerTests
    {
        private static Document Expected
        {
            get
            {
                var gettingStarted = new Document.Entry()
                                         {
                                             ThumbExists = false,
                                             Path = new Uri("/GettingStarted.pdf", UriKind.Relative),
                                             ClientMtime =
                                                 new DateTimeOffset(2014, 1, 8, 18, 0, 54, TimeSpan.Zero),
                                             Bytes = 249159
                                         };
                var task = new Document.Entry()
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
                                   new Dictionary<string, Document.Entry>()
                                       {
                                           ["/gettingstarted.pdf"] = gettingStarted,
                                           ["/task.jpg"] = task
                                       }
                           };
            }
        }

        [Fact]
        public void Almost_empty_JSON_should_deserialize_as_such()
        {
            var sut = new DocumentDeserializer();

            var result = sut.Deserialize(Inputs.Empty);

            result.HasTitle.Should().NotHaveValue();
            result.Title.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Original_structure_indirect_deserialization()
        {
            var sut = new DocumentDeserializer();

            var result = sut.Deserialize(Inputs.Original);

            result.ShouldBeEquivalentTo(Expected);
        }

        [Fact]
        public void AlternativeJsonDeserializesDirectly()
        {
            var response = new RestResponse();
            response.Content = Inputs.Alternative;
            response.ContentType = "text/json";
            var deserializer = new JsonDeserializer();
            deserializer.Culture = CultureInfo.InvariantCulture;
            deserializer.DateFormat = DocumentDeserializer.DateFormat;
            var result = deserializer.Deserialize<Document>(response);
            result.ShouldBeEquivalentTo(Expected);
        }

        [Fact]
        public void DateFormatTest()
        {
            var format = DocumentDeserializer.DateFormat;
            var now = new DateTimeOffset(2015, 9, 24, 21, 16, 37, TimeSpan.FromHours(2));
            var actual = now.ToString(format, CultureInfo.InvariantCulture);
            Assert.Equal("Thu, 24 Sep 2015 21:16:37+02:00", actual);
        }
    }
}