using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CrossOrigin.WebService.Models.DbEntities;
using Microsoft.EntityFrameworkCore;


namespace CrossOrigin.WebService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {

            //Add database services.
            var connectionString = this.Configuration.GetConnectionString("dbConn");
            services.AddDbContext<PhoneBookContext>(options => options.UseSqlServer(connectionString));

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            //Add CORS services.
            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));
            //});

            services.AddCors(
                options => options.AddPolicy("AllowCors",
                builder =>
                {
                    builder
                    //.WithOrigins("http://localhost:4456") //AllowSpecificOrigins;
                    //.WithOrigins("http://localhost:4456", "http://localhost:4457") //AllowMultipleOrigins;
                    .AllowAnyOrigin() //AllowAllOrigins;

                    //.WithMethods("GET") //AllowSpecificMethods;
                    //.WithMethods("GET", "PUT") //AllowSpecificMethods;
                    //.WithMethods("GET", "PUT", "POST") //AllowSpecificMethods;
                    .WithMethods("GET", "PUT", "POST", "DELETE") //AllowSpecificMethods;
                                                                 //.AllowAnyMethod() //AllowAllMethods;

                    //.WithHeaders("Accept", "Content-type", "Origin", "X-Custom-Header");  //AllowSpecificHeaders;
                    .AllowAnyHeader(); //AllowAllHeaders;
                })
            );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();

            //Enable CORS policy "AllowCors"
            app.UseCors("AllowCors");

        }
    }
}
