using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Weather
{
    public class WeatherClassifierWorker : BackgroundService
    {
        private SqueezeNetModel _squeezeNetModel;
        private readonly List<string> _labels = new List<string>();

        private readonly ILogger<WeatherClassifierWorker> _logger;
        private readonly CommandLineOptions _options;

        public WeatherClassifierWorker(ILogger<WeatherClassifierWorker> logger, CommandLineOptions options)
        {
            _logger = logger;
            _options = options;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _logger.LogInformation("Service started");

            if (!Directory.Exists(_options.Path))
            {
                _logger.LogError($"Directory "{ _options.Path}
                " does not exist.");
                return;
            }

            var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Load labels from JSON
            foreach (var kvp in JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(Path.Combine(rootDir, "Labels.json"))))
            {
                _labels.Add(kvp.Value);
            }

            _squeezeNetModel = SqueezeNetModel.CreateFromFilePath(Path.Combine(rootDir, "squeezenet1.0-9.onnx"));


            _logger.LogInformation($"Listening for images created in "{ _options.Path}
            "...");

            using FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = _options.Path
            };
            foreach (var extension in _options.Extensions)
            {
                watcher.Filters.Add($"*.{extension}");
            }
            watcher.Created += async (object sender, FileSystemEventArgs e) =>
            {
                await Task.Delay(1000);
                await ProcessFileAsync(e.FullPath, _options.Confidence);
            };

            watcher.EnableRaisingEvents = true;


            var tcs = new TaskCompletionSource<bool>();
            stoppingToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            await tcs.Task;

            _logger.LogInformation("Service stopped");

            if (results[0].p >= confidence)
            {
                MoveFileToFolder(filePath, _labels[results[0].index]);

                return true; // Success
            }

            return false; // Not enough confidence or error


        }

        private void MoveFileToFolder(string filePath, string folderName)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var destinationDirectory = Path.Combine(directory, folderName);

            Directory.CreateDirectory(destinationDirectory);

            File.Move(filePath, Path.Combine(destinationDirectory, fileName), false);
        }


        public static async Task GetWeather()
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

    }
}
