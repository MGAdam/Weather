using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Weather
{
    public class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async (opts) =>
                {
                    await CreateHostBuilder(args, opts).Build().RunAsync();
                    return 0;
                },
                errs => Task.FromResult(-1)); // Invalid arguments
        }

        public static IHostBuilder CreateHostBuilder(string[] args, CommandLineOptions opts) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(configureLogging => configureLogging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Information))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(opts);
                    services.AddHostedService<ImageClassifierWorker>()
                        .Configure<EventLogSettings>(config =>
                        {
                            config.LogName = "Image Classifier Service";
                            config.SourceName = "Image Classifier Service Source";
                        });
                }).UseWindowsService();

        static async Task Main()
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
