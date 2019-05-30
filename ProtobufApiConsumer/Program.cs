using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufApiConsumer
{
    using SampleModel;

    class Program
    {
        private const string ProtobufApiUrl = "https://localhost:44316/api/values";
        private const string ProtobufHeader = "application/x-protobuf";

        static async Task Main(string[] args)
        {
            var result = await PostStreamDataToServerAsync();

            foreach (var item in await CallServerAsync())
                Console.WriteLine($"Id: {item.Id}, IsComplete: {item.IsComplete}, Name: {item.Name}");

            Console.ReadKey();
        }

        static async Task<List<ValueItem>> CallServerAsync()
        {
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, ProtobufApiUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ProtobufHeader));

            var result = await client.SendAsync(request);
            return ProtoBuf.Serializer.Deserialize<List<ValueItem>>(await result.Content.ReadAsStreamAsync());
        }

        static async Task<ValueItem> PostStreamDataToServerAsync()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ProtobufHeader));

            var request = new HttpRequestMessage(HttpMethod.Post, ProtobufApiUrl);

            var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(stream, new ValueItem
            {
                Id = RandomNumber(),
                IsComplete = RandomBoolean(),
                Name = RandomString(100),
            });

            request.Content = new ByteArrayContent(stream.ToArray());
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(ProtobufHeader);

            var responseForPost = await client.SendAsync(request);
            return ProtoBuf.Serializer.Deserialize<ValueItem>(await responseForPost.Content.ReadAsStreamAsync());
        }

        static int RandomNumber() => new Random().Next(int.MinValue, int.MaxValue);

        static bool RandomBoolean() => (new Random().Next(int.MinValue, int.MaxValue) % 2 == 0);

        static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
