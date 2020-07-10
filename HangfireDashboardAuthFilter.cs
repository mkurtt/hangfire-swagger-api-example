using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;

namespace HangfireSwaggerApi
{
	/// <summary>
	/// Class to filter Authorization of hangfire UI. Hangfire arayuzune girme yetkisini saglayan class.
	/// </summary>
	internal class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
	{
		public HangfireDashboardAuthFilter()
		{
		}

		[Authorize]
		public bool Authorize([NotNull] DashboardContext context)
		{
			// Check Authorization
			return true;
		}
	}
}