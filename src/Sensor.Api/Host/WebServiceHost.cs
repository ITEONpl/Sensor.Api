using System;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Sensor.Api.Host
{
    public class WebServiceHost : IWebServiceHost
    {
        private readonly IWebHost _webHost;

        public WebServiceHost(IWebHost webHost)
        {
            _webHost = webHost;
        }

        public void Run()
        {
            _webHost.Run();
        }

        public IWebHost GetWebHost()
        {
            //Run();
            return _webHost;
        }

        public static Builder Create<TStartup>(string name = "", string[] args = null) where TStartup : class
        {
            if (string.IsNullOrWhiteSpace(name))
                name = $"Adm Service: {typeof(TStartup).Namespace.Split('.').Last()}";
            Console.Title = name;
            var webHost = new WebHostBuilder()
                .UseStartup<TStartup>()
                .UseKestrel()
                .UseIISIntegration()
                .ConfigureServices(services => services.AddAutofac())
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build())
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var env = builderContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                    config.AddEnvironmentVariables();
                    if (args != null)
                        config.AddCommandLine(args);
                })
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsEnvironment("local");
                })
                .UseUrls("http://*:5000")
                .Build();

            return new Builder(webHost);
        }

        public abstract class BuilderBase
        {
            public abstract WebServiceHost Build();
        }

        public class Builder : BuilderBase
        {
            private readonly IWebHost _webHost;

            //            private IBusClient _bus;
            private IResolver _resolver;

            public Builder(IWebHost webHost)
            {
                _webHost = webHost;
            }

            public Builder UseAutofac(ILifetimeScope scope)
            {
                _resolver = new AutofacResolver(scope);

                return this;
            }

            //            public BusBuilder UseRabbitMq(string queueName = null)
            //            {
            //                _bus = _resolver.Resolve<IBusClient>();
            //
            //                return new BusBuilder(_webHost, _bus, _resolver, queueName);
            //            }

            public override WebServiceHost Build()
            {
                return new WebServiceHost(_webHost);
            }
        }
    }
}