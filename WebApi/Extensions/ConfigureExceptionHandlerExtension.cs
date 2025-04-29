using Microsoft.AspNetCore.Diagnostics;
using ServiceLayer.Exceptions;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace WebApi.Extensions
{
	public static class ConfigureExceptionHandlerExtension
	{
		public static void ConfigureExceptionHandler(this WebApplication app, ILogger<Program> logger)
		{
			app.UseExceptionHandler(builder =>
			{
				builder.Run(async context =>
				{
					var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
					var statusCode = HttpStatusCode.InternalServerError;
					var errorMessage = exception?.Message ?? "An unexpected error occurred."; // Default error message

					if (exception != null)
					{
						if(exception is UserNotFoundException)
						{
							statusCode = HttpStatusCode.NotFound;
						}
						if (exception is UnauthorizedAccessException)
						{
							statusCode = HttpStatusCode.Unauthorized;
						}
						else if (exception is ArgumentException)
						{
							statusCode = HttpStatusCode.BadRequest;
						}
						else if (exception is KeyNotFoundException)
						{
							statusCode = HttpStatusCode.NotFound;
						}
						else if (exception is ValidationException)  // Validation exception handled here
						{
							statusCode = HttpStatusCode.BadRequest;
							errorMessage = "Validation failed: " + exception.Message;
						}

						logger.LogError(exception.Message);
					}

					context.Response.StatusCode = (int)statusCode;
					context.Response.ContentType = MediaTypeNames.Application.Json;
					await context.Response.WriteAsync(JsonSerializer.Serialize(
						new
						{
							StatusCode = context.Response.StatusCode,
							Message = errorMessage,
							Title = "Error Occurred"
						}
					));
				});
			});
		}
	}
}
