using DataAccessLayer.Extensions;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using ServiceLayer.Extensions;
using System.Text.Json.Serialization;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using WebApi.Configurations.ColumnWriters;
using Serilog.Context;
using Microsoft.AspNetCore.HttpLogging;
using WebApi.Extensions;
using FluentValidation.AspNetCore;
using ServiceLayer.Filters;

namespace WebApi
{
	public class Program
	{
		public async static Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers(opt=>opt.Filters.Add<ValidationFilter>()).AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
				options.JsonSerializerOptions.WriteIndented = true;
			}); ;
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		
				builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Mpotify", Version = "v1", Description = "Swagger client" });

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

			//SqlColumn sqlColumn = new SqlColumn();

			//sqlColumn.ColumnName = "UserName";

			//sqlColumn.DataType = System.Data.SqlDbType.NVarChar;

			//sqlColumn.PropertyName = "UserName";

			//sqlColumn.DataLength = 50;

			//sqlColumn.AllowNull = true;

			//ColumnOptions columnOpt = new ColumnOptions();

			//columnOpt.Store.Remove(StandardColumn.Properties);

			//columnOpt.Store.Add(StandardColumn.LogEvent);

			//columnOpt.AdditionalColumns = new Collection<SqlColumn> { sqlColumn };


			Logger log = new LoggerConfiguration()
				.WriteTo.Console()
				.WriteTo.File("logs/log.txt")
				//.WriteTo.MSSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),"logs",autoCreateSqlTable:true,columnOptions:columnOpt)
				.WriteTo.Seq(builder.Configuration["Seq:ServerUrl"])
				.Enrich.FromLogContext()
				.Enrich.With<CustomUsernameColumn>()
				.MinimumLevel.Information()
				.CreateLogger(); 

			builder.Host.UseSerilog(log);
			builder.Services.AddHttpLogging(logging =>
			{
				logging.LoggingFields = HttpLoggingFields.All;
				logging.RequestHeaders.Add("sec-ch-ua");
				logging.MediaTypeOptions.AddText("application/javascript");
				logging.RequestBodyLogLimit = 4096;
				logging.ResponseBodyLogLimit = 4096;
				logging.CombineLogs = true;
			});
			builder.Services.AddCors();
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
			app.ConfigureExceptionHandler(app.Services.GetRequiredService<ILogger<Program>>());
			app.UseSerilogRequestLogging();
			app.UseHttpLogging();
			app.UseCors(opt => {
				opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:5174", "http://localhost:5173");
			});
			app.UseStaticFiles();


			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();

			app.Use(async (context, next) =>
			{
				var username=context?.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
				LogContext.PushProperty("UserName",username);
				await next();

			});

			app.MapControllers();

			app.Run();
		}
	}
}
