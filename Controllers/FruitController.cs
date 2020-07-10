using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using HangfireSwaggerApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace HangfireSwaggerApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FruitController : ControllerBase
	{
		private readonly IMongoCollection<Fruit> fruitCollection;

		private BGJobDetails bgJob { get; set; }
		private Logger logger { get; set; }

		public FruitController(IMongoDatabase database)
		{
			fruitCollection = database.GetCollection<Fruit>("Fruits");

			bgJob = new BGJobDetails();
			logger = new Logger(database);
		}

		/// <summary>
		/// Get all fruits as list. List turunce butun Fruitleri al.
		/// </summary>
		/// <returns></returns>
		[HttpGet("get")]
		public List<Fruit> GetFruit()
		{
			bgJob.Id = BackgroundJob.Enqueue(() => Console.Write("Listing Fruits, "));
			bgJob.Title = "List Request";
			bgJob.Description = "Listed fruits";

			BackgroundJob.ContinueJobWith(bgJob.Id, () => logger.HFLogger(bgJob));
			return fruitCollection.AsQueryable().ToList();
		}


		/// <summary>
		/// insert fruit into the database. Veritabanina meyve ekle.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[HttpGet("insert/{name}")]
		public bool InsertFruit(string name)
		{
			fruitCollection.InsertOne(new Fruit
			{
				Name = name
			});

			bgJob.Id = BackgroundJob.Enqueue(() => Console.Write("insert succeed, "));
			bgJob.Title = "Insert";
			bgJob.Description = name + " inserted";

			BackgroundJob.ContinueJobWith(bgJob.Id, () => logger.HFLogger(bgJob));
			return true;
		}

		/// <summary>
		/// update fruit.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="newName"></param>
		/// <returns></returns>
		[HttpGet("update/{name}_{newName}")]
		public bool Update(string name, string newName)
		{
			var filter = Builders<Fruit>.Filter.Eq("Name", name);
			var update = Builders<Fruit>.Update.Set("Name", newName);
			var result = fruitCollection.UpdateOne(filter, update);

			if (result.ModifiedCount == 0)
			{
				bgJob.Id = BackgroundJob.Enqueue(() => Console.Write("update failed, "));
				bgJob.Title = "Update";
				bgJob.Description = name + " update to " + newName + " failed";
			}
			else
			{
				bgJob.Id = BackgroundJob.Enqueue(() => Console.Write("update succeed, "));
				bgJob.Title = "Update";
				bgJob.Description = name + " Updated to " + newName;
			}
			BackgroundJob.ContinueJobWith(bgJob.Id, () => logger.HFLogger(bgJob));
			return true;
		}

		/// <summary>
		/// remove fruit from the database. Veritabanindan meyve cikart
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[HttpGet("remove/{name}")]
		public bool Remove(string name)
		{
			var filter = Builders<Fruit>.Filter.Eq("Name", name);
			var result = fruitCollection.DeleteOne(filter);

			if (result.DeletedCount == 0)
			{
				bgJob.Id = BackgroundJob.Enqueue(() => Console.Write(" update failed, "));
				bgJob.Title = "Remove";
				bgJob.Description = name + " removing failed";
			}
			else
			{
				bgJob.Id = BackgroundJob.Enqueue(() => Console.Write(" remove succeed, "));
				bgJob.Title = "Remove";
				bgJob.Description = name + " removed";
			}
			BackgroundJob.ContinueJobWith(bgJob.Id, () => logger.HFLogger(bgJob));
			return true;
		}
	}
}
