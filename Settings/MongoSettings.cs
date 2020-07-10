using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireSwaggerApi.Settings
{
	public class MongoSettings
	{
		public string Environment { get; set; }
		public string ConnectionString { get; set; }
		public string Database { get; set; }

	}
}
