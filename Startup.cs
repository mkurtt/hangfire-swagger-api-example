using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using HangfireSwaggerApi.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace HangfireSwaggerApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMongoSettings(Configuration);


			//gets data from appsettings. Appsettings ten veri alir.
			services.AddSingleton(sp =>
			{
				var swaggerSettings = new SwaggerSettings();
				Configuration.GetSection(nameof(SwaggerSettings)).Bind(swaggerSettings);
				return swaggerSettings;
			});


			services.AddControllers();


			services.AddCustomHangfire(Configuration);


			// SwaggerUI generator settings
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "TheCodeBuzz-Service", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}



			// Swagge options 
			var swaggerOptions = app.ApplicationServices.GetRequiredService<SwaggerSettings>();

			app.UseSwagger(option =>
			{
				option.RouteTemplate = swaggerOptions.JsonRoute;
			});

			app.UseSwaggerUI(opt =>
			{
				opt.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
			});



			app.UseHangfireDashboard("/hangfire", new DashboardOptions()
			{
				AppPath = "localhost:5001/swagger",
				DashboardTitle = "IUC Hangfire",

				Authorization = new[] { new HangfireDashboardAuthFilter() }
			});
			app.UseHangfireServer();


			// hangfire, background minutely repeating job. Her dadike gerceklesen is ornegi
			RecurringJob.AddOrUpdate("HFCounter", () => ExampleFunctions.countSpammer(), Cron.Minutely);


			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
