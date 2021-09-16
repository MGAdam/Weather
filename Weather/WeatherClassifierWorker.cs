using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Weather
{
   public class WeatherClassifierWorker : BackgroundService
    {
        static readonly HttpClient client = new HttpClient();
        private SqueezeNetModel _squeezeNetModel;
        private readonly List<string> _labels = new List<string>();

        private readonly ILogger<WeatherClassifierWorker> _logger;
        private readonly CommandLineOptions _options;

        public WeatherClassifierWorker(ILogger<WeatherClassifierWorker> logger, CommandLineOptions options)
        {
            _logger = logger;
            _options = options;
        }

        static async Task GetWeather()
        {

            var locator = new Windows.Devices.Geolocation.Geolocator();

            var position = (await locator.GetGeopositionAsync()).Coordinate.Point.Position;

            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://api.openweathermap.org/data/2.5/weather?lat={position.Latitude}&lon={position.Longitude}&appid=171dd6e6b0fe8d04a0fd21a1a4330d49&units=metric");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Models m = JsonConvert.DeserializeObject<Models>(responseBody);



                Console.WriteLine($"Ort: {m.Name}, Tempmax: {m.main.TempMax}, Tempmin: {m.main.TempMin}");
            }

            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);

            }



        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            stoppingToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            await tcs.Task;

            _logger.LogInformation("Service stopped");
        }
    }
}
