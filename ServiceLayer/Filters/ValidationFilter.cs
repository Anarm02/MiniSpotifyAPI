using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Filters
{
	public class ValidationFilter : IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!context.ModelState.IsValid)
			{
				var errors = context.ModelState.Where(x => x.Value.Errors.Any())
					.ToDictionary(x => x.Key, x => x.Value.Errors.Select(x => x.ErrorMessage)).ToArray();
				return;
			}
			await next();
		}
	}
}
