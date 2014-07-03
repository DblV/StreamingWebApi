using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StreamingConsumerConsole
{
    public class Run
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("About to update image, press a key...");
            Console.ReadKey();

            var updateTask = UpdateImageAsync();
            var response = updateTask.Result;

            Console.WriteLine("Image sent, response was " + response.StatusCode);
            Console.ReadKey();
        }

        private static async Task<HttpResponseMessage> UpdateImageAsync()
        {
            HttpClient client = new HttpClient();

            HttpContent content = new StreamContent(File.Open("images\\sample.jpg", FileMode.Open));
            Task<HttpResponseMessage> responseTask = client.PostAsync("http://holymissingsitebatman.com/api/streaming/updateimage/1", content);

            Console.WriteLine("Sending image");

            return await responseTask;
        }
    }
}
