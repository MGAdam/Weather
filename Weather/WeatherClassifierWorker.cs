using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Weather
{
    public class WeatherClassifierWorker : BackgroundService
    {

        private readonly ILogger<WeatherClassifierWorker> _logger;


        static readonly HttpClient client = new HttpClient();

        public WeatherClassifierWorker(ILogger<WeatherClassifierWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _logger.LogInformation("Service started");


            var locator = new Windows.Devices.Geolocation.Geolocator();

            var position = (await locator.GetGeopositionAsync()).Coordinate.Point.Position;

            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://api.openweathermap.org/data/2.5/weather?lat={position.Latitude}&lon={position.Longitude}&appid=171dd6e6b0fe8d04a0fd21a1a4330d49&units=metric");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Models m = JsonConvert.DeserializeObject<Models>(responseBody);


                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText($"Ort: {m.Name}")
                    .AddText($"Tempmax: {m.main.TempMax}")
                    .AddText($"Tempmin: {m.main.TempMin}")
                    .Show();


                Console.WriteLine($"Ort: {m.Name}, Tempmax: {m.main.TempMax}, Tempmin: {m.main.TempMin}");
            }

            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);

            }


            var tcs = new TaskCompletionSource<bool>();
            stoppingToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            await tcs.Task;

            _logger.LogInformation("Service stopped");

        }


    }
}
