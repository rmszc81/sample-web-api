using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using MessagePack;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace MessagePackApiConsumer
{
    using SampleModel;
    
    class Program
    {
        private const string MessagePackApiUrl = "https://localhost:44316/api/values";
        private const string MessagePackHeader = "application/x-msgpack";

        static async Task Main(string[] args)
        {
            var disco = await new HttpClient().GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await new HttpClient().RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine();
            Console.WriteLine();

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("https://localhost:44316/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            var sw = Stopwatch.StartNew();

            Console.WriteLine("Posting data to server...");

            var result = await PostStreamDataToServerAsync(client);

            Console.WriteLine("Done! Elapsed time: {0}.", sw.Elapsed);

            Console.WriteLine();

            Console.WriteLine("Reading data from server...");

            foreach (var item in await CallServerAsync(client))
                Console.WriteLine($"Id: {item.Id}, IsComplete: {item.IsComplete}, Name: {item.Name}");

            Console.WriteLine("Done! Elapsed time: {0}.", sw.Elapsed);

            Console.WriteLine();
            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }

        static async Task<List<ValueItem>> CallServerAsync(HttpClient httpClient)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, MessagePackApiUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MessagePackHeader));

            var result = await httpClient.SendAsync(request);
            return MessagePackSerializer.Deserialize<List<ValueItem>>(await result.Content.ReadAsStreamAsync());
        }

        static async Task<ValueItem> PostStreamDataToServerAsync(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MessagePackHeader));

            var request = new HttpRequestMessage(HttpMethod.Post, MessagePackApiUrl);

            var stream = new MemoryStream();
            MessagePackSerializer.Serialize(stream, new ValueItem
            {
               Id = RandomNumber(),
               IsComplete = RandomBoolean(),
               Name = RandomString(100)
            });

            request.Content = new ByteArrayContent(stream.ToArray());
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(MessagePackHeader);

            var responseForPost = await httpClient.SendAsync(request);
            return MessagePackSerializer.Deserialize<ValueItem>(await responseForPost.Content.ReadAsStreamAsync());
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
