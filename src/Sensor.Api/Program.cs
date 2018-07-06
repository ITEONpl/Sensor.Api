using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Sensor.Api.Host;
using Serilog;
using Serilog.Events;

namespace Sensor.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var host = BuildWebHost(args);
                Log.Information("Starting web host");
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebServiceHost
                .Create<Startup>(args: args)
                .Build()
                .GetWebHost();
        }
    }
}