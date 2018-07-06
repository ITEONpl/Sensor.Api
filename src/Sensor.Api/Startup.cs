using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;

namespace Sensor.Api
{
    public class Startup
    {
        private readonly Serilog.ILogger _logger = Log.Logger;
        private static readonly string[] Headers = {"X-Operation", "X-Resource", "X-Total-Count"};
        private IHostingEnvironment _env;

        public Startup(IConfiguration configuration)
        {
            _logger.Information($"Application Startup at {DateTime.UtcNow} UTC");
            Configuration = configuration;
        }

        public IContainer Container { get; private set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://192.168.1.200:9200"))
                //{
                //    AutoRegisterTemplate = true
                //})
                .CreateLogger();

            Log.Logger = logger;

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "API",
                    Version = "v1",
                    Description = "Description"
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", cors =>
                    cors.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders(Headers));
            });

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
                .AsImplementedInterfaces();
            builder.Populate(services);
            //builder.AddRabbitMq();


            // Adding Auto Mapper
            //services.AddAutoMapper();

            Container = builder.Build();

            return new AutofacServiceProvider(Container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            //if (env.IsDevelopment())
            //    app.UseDeveloperExceptionPage();
            //else
            //    app.UseHsts();

            _env = env;
            // Add Serilog to the logging pipeline
            loggerFactory.AddSerilog();
            loggerFactory.AddConsole();

            if (env.IsDevelopment() || env.EnvironmentName == "local")
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseBrowserLink();
                loggerFactory.AddDebug();
            }

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseMvc();
            applicationLifetime.ApplicationStopped.Register(() => Container.Dispose());
            // Ensure any buffered events are sent at shutdown
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs"); });
            //app.UseSwaggerUi3(typeof(Startup).Assembly);
        }
    }
}