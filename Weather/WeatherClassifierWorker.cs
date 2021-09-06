using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather
{
   public class WeatherClassifierWorker : BackgroundService
    {
        private SqueezeNetModel _squeezeNetModel;
        private readonly List<string> _labels = new List<string>();

        private readonly ILogger<ImageClassifierWorker> _logger;
        private readonly CommandLineOptions _options;

        public WeatherClassifierWorker(ILogger<ImageClassifierWorker> logger, CommandLineOptions options)
        {
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ...
        }
    }
}
