namespace Deserializer
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    using RestSharp;
    using RestSharp.Deserializers;

    using Xunit;
    using Xunit.Abstractions;

    public class LoadTests
    {
        private readonly ITestOutputHelper output;

        public LoadTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(2, 500)]
        [InlineData(2, 5000)]
        [InlineData(10, 500)]
        [InlineData(10, 5000)]
        [InlineData(20, 500)]
        [InlineData(20, 5000)]
        public void AlternativeLoadTest(int numberOfEntries, int numberOfLoops)
        {
            string jsonFinal = GenerateAlternativeJsonString(numberOfEntries);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = new RestResponse();
            response.Content = jsonFinal;
            response.ContentType = "text/json";
            var deserializer = new JsonDeserializer();
            deserializer.Culture = CultureInfo.InvariantCulture;
            deserializer.DateFormat = DocumentDeserializer.DateFormat;
            for (int i = 0; i < numberOfLoops; i++)
            {
                deserializer.Deserialize<Document>(response);
            }

            stopwatch.Stop();
            this.output.WriteLine(
                $"Alternative format with {numberOfEntries} entries after {numberOfLoops} of loops took {stopwatch.Elapsed}");
        }

        [Theory]
        [InlineData(2, 500)]
        [InlineData(2, 5000)]
        [InlineData(10, 500)]
        [InlineData(10, 5000)]
        [InlineData(20, 500)]
        [InlineData(20, 5000)]
        public void OriginalLoadTest(int numberOfEntries, int numberOfLoops)
        {
            string jsonFinal = GenerateOriginalJsonString(numberOfEntries);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var deserializer = new DocumentDeserializer();
            for (int i = 0; i < numberOfLoops; i++)
            {
                deserializer.Deserialize(jsonFinal);
            }

            stopwatch.Stop();

            this.output.WriteLine(
                $"Original format with {numberOfEntries} entries after {numberOfLoops} of loops took {stopwatch.Elapsed}");

        }

        private static string GenerateAlternativeJsonString(int numberOfEntries)
        {
            var header = @"{
  'has_title': true,
  'title': 'GoodLuck',
  'entries': {";
            var item = @"
    '/test{0}.pdf': {{
      'thumb_exists': false,
      'path': '/GettingStarted.pdf',
      'client_mtime': 'Wed, 08 Jan 2014 18:00:54+0000',
      'bytes': 249159
    }},";
            var footer = @"
    '/task-last.jpg': {
      'thumb_exists': true,
      'path': '/Task.jpg',
      'client_mtime': 'Tue, 14 Jan 2014 05:53:57+0000',
      'bytes': 207696
    }
  }
}";
            return GenerateJsonString(numberOfEntries, header, item, footer);
        }

        private static string GenerateOriginalJsonString(int numberOfEntries)
        {
            var header = @"{
  'has_title': true,
  'title': 'GoodLuck',
  'entries': [";
            var item = @"[
    '/test{0}.pdf', {{
      'thumb_exists': false,
      'path': '/GettingStarted.pdf',
      'client_mtime': 'Wed, 08 Jan 2014 18:00:54+0000',
      'bytes': 249159
    }}],";
            var footer = @"[
    '/task-last.jpg', {
      'thumb_exists': true,
      'path': '/Task.jpg',
      'client_mtime': 'Tue, 14 Jan 2014 05:53:57+0000',
      'bytes': 207696
    }
  ]]
}}";
            return GenerateJsonString(numberOfEntries, header, item, footer);
        }

        private static string GenerateJsonString(int numberOfEntries, string header, string item, string footer)
        {
            var sb = new StringBuilder();
            sb.Append(header);
            for (int i = 0; i < numberOfEntries; i++)
            {
                sb.AppendFormat(item, i);
            }
            sb.Append(footer);
            var jsonFinal = sb.ToString().Replace("'", "\"");
            return jsonFinal;
        }
    }
}