using HangfireSwaggerApi.Models;
using HangfireSwaggerApi.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo;
using Hangfire;
using Hangfire.SqlServer;

namespace HangfireSwaggerApi
{
	public static class DependencyManager
	{
        /// <summary>
        /// Get and Create MongoDB options based on appsettings. Appsettings ten alinan veri ile MongoDB ayarlarini yapar. 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddMongoSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MongoSettings>(config.GetSection("MongoSettings"));

            services.AddSingleton(sp =>
            {
                return sp.GetRequiredService<IOptions<MongoSettings>>().Value;
            });

            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<MongoSettings>();
                return new MongoClient(settings.ConnectionString);
            });

            services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<MongoClient>();
                var settings = sp.GetRequiredService<MongoSettings>();
                return client.GetDatabase(settings.Database);
            });

            return services;
        }

        /// <summary>
        /// Adds custom setted Mongo Identity Provider, ozel ayarlanmis Mongo Identity Provider ekler.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMongoIdentity(this IServiceCollection services)
        {
            var settings = services.BuildServiceProvider().GetRequiredService<MongoSettings>();

            services.AddIdentityMongoDbProvider<User>(
                idtOptions =>
                {
                    idtOptions.Password.RequireDigit = true;
                    idtOptions.Password.RequireLowercase = true;
                    idtOptions.Password.RequireNonAlphanumeric = false;
                    idtOptions.Password.RequiredLength = 4;
                },
                mongoIdnOptions =>
                {
                    mongoIdnOptions.ConnectionString = settings.ConnectionString + "/" + settings.Database;
                    mongoIdnOptions.UsersCollection = "AppUsers";
                }
                );

            return services;
        }

		/// <summary>
		/// Adds custom setted Hangfire, ozel ayarlanmis Hangfire i ekler.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IServiceCollection AddCustomHangfire(this IServiceCollection services, IConfiguration config)
		{
			services.AddHangfire(configuration => configuration
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseSqlServerStorage(config.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
				{
					CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
					SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
					QueuePollInterval = TimeSpan.Zero,
					UseRecommendedIsolationLevel = true,
					DisableGlobalLocks = true
				}));

			return services;
		}
	}
}
