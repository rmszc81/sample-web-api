using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace ProtobufApiConsumer
{

    using SampleModel;

    class Program
    {
        private const string ProtobufApiUrl = "https://localhost:44316/api/values";
        private const string ProtobufHeader = "application/x-protobuf";

        static async Task Main(string[] args)
        {
            var disco = await new HttpClient().GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            TokenResponse tokenResponse = null;

            Console.WriteLine("Choose the authentication type:");
            Console.WriteLine("1: user name and password.");
            Console.WriteLine("2: client id/secret only.");
            Console.WriteLine();
            var option = Console.ReadKey();

            Console.WriteLine();

            if (option.KeyChar == (char)'1')
            {
                // request token by username and password
                tokenResponse = await new HttpClient().RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,

                    ClientId = "client",
                    ClientSecret = "secret",
                    UserName = "admin",
                    Password = "password",
                    Scope = "api1"
                });
            }
            else if (option.KeyChar == (char)'2')
            {
                // request token by client id/secret
                tokenResponse = await new HttpClient().RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,

                    ClientId = "client",
                    ClientSecret = "secret",
                    Scope = "api1"
                });
            }
            else
            {
                Console.WriteLine("Wrong option selected. Aborting.");
                Console.WriteLine("Press any key to finish.");
                Console.ReadKey();

                return;
            }

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
            var request = new HttpRequestMessage(HttpMethod.Get, ProtobufApiUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ProtobufHeader));

            var result = await httpClient.SendAsync(request);
            return ProtoBuf.Serializer.Deserialize<List<ValueItem>>(await result.Content.ReadAsStreamAsync());
        }

        static async Task<ValueItem> PostStreamDataToServerAsync(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ProtobufHeader));

            var request = new HttpRequestMessage(HttpMethod.Post, ProtobufApiUrl);

            var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(stream, new ValueItem
            {
                Id = RandomNumber(),
                IsComplete = RandomBoolean(),
                Name = RandomString(100)
            });

            request.Content = new ByteArrayContent(stream.ToArray());
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(ProtobufHeader);

            var responseForPost = await httpClient.SendAsync(request);
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
