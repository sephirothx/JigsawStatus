using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JigsawStatus
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service started");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service stopped");
            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Checks for process status every 5 seconds, if the process is not running attempts to start it again.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var proc = Process.GetProcessesByName("JigsawBot");

                if (proc.Length == 0)
                {
                    _logger.LogInformation("JigsawBot is down. Restarting...");

                    try
                    {
                        using (var process = new Process())
                        {
                            process.StartInfo = new ProcessStartInfo
                            {
                                FileName        = @"",
                                UseShellExecute = true,
                                CreateNoWindow  = false
                            };

                            process.Start();
                        }

                        _logger.LogInformation("Bot restarted");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unable to restart the bot. Retrying in 5 seconds");
                    }
                }

                await Task.Delay(5 * 1000, stoppingToken);
            }
        }
    }
}
