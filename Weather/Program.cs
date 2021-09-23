using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.EventLog;
using Newtonsoft.Json;

namespace Weather
{
    public class Program
    {
        static async Task<int> Main(string[] args)
        {

            await CreateHostBuilder(args).Build().RunAsync();
            return 0;

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<WeatherClassifierWorker>()
                        .Configure<EventLogSettings>(config =>
                        {
                            config.LogName = "Weather Classifier Service";
                            config.SourceName = "Weather Classifier Service Source";
                        });
                }).UseWindowsService();


    }

}
