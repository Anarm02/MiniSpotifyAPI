using DataAccessLayer.Extensions;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using ServiceLayer.Extensions;
using System.Text.Json.Serialization;

namespace WebApi
{
	public class Program
	{
		public async static Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
				options.JsonSerializerOptions.WriteIndented = true;
			}); ;
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		
				builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo() { Title = "DostProject", Version = "v1", Description = "Swagger client" });

				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Bearer space token"
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new List<string>()
		}
	});
				
			});
			builder.Services.AddDalExtension(builder.Configuration);
			builder.Services.AddServiceLayer(builder.Configuration);
			var app = builder.Build();
		    using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
				var roles = new[] { "User", "Admin", "SuperAdmin" };
				foreach(var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
						await roleManager.CreateAsync(new AppRole() { Name=role});
				}
			}
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
				app.UseDeveloperExceptionPage();
			}
			app.UseStaticFiles();
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
