using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireSwaggerApi
{
	public static class ExampleFunctions
	{
		static int count = 0;

		/// <summary>
		/// Counter. Sayac.
		/// </summary>
		public static void countSpammer()
		{
			count++;
			Console.WriteLine(count);
		}
	}
}
