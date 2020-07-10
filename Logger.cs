using HangfireSwaggerApi.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireSwaggerApi
{
	public class Logger
	{
		private readonly IMongoCollection<Log> logCollection;

		public Logger(IMongoDatabase database)
		{
			logCollection = database.GetCollection<Log>("Logs");
		}

		/// <summary>
		/// Logs into the application's database using hangfire.
		/// Hangfire ile uygulamanin veritabanina loglama yapar.
		/// </summary>
		/// <param name="bgJob"> Gets log title and details form the job that's done. Log basligini ve aciklamasini yapilan isten alir.</param>
		public void HFLogger(BGJobDetails bgJob)
		{
			logCollection.InsertOne(new Log()
			{
				Title = bgJob.Title,
				Description = bgJob.Description
			});

			Console.WriteLine("Logging Successful");
		}
	}
}
