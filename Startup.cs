using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using SmartEnergy.ConfigClass;
using SmartEnergy.Middleware;
using StackifyLib;
using StackifyLib.CoreLogger;

namespace SmartEnergy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StackifyLib.Utils.StackifyAPILogger.LogEnabled = true;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartEnergy", Version = "v1" });
            });
            services.Configure<PythonFile>(Configuration.GetSection("PythonFile"));
            services.Configure<DirectorySetup>(Configuration.GetSection("DirectorySetup"));
            services.Configure<Credentials>(Configuration.GetSection("Credentials"));
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseExceptionHandlingMiddleware();

            app.UseSwagger();

            app.UseSerilogRequestLogging();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartEnergy V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            loggerFactory.AddStackify();
            app.ConfigureStackifyLogging(Configuration);
        }
    }
}
