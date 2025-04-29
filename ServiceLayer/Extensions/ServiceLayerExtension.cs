using DataAccessLayer.Context;
using EntityLayer.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Filters;
using ServiceLayer.Helpers.ImageHelpers;
using ServiceLayer.RedisCache;
using ServiceLayer.Services.Abstract;
using ServiceLayer.Services.Concrete;
using ServiceLayer.Validators.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Extensions
{
	public static class ServiceLayerExtension
	{


		public static void AddServiceLayer(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<ITokenService, TokenService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<ISongService, SongService>();
			services.AddScoped<IPlaylistService, PlaylistService>();
			services.AddScoped<ISearchService, SearchService>();
			services.AddScoped<IImageHelper, ImageHelper>();
			services.Configure<RedisCacheSettings>(
	configuration.GetSection("RedisCacheSettings"));

			services.AddSingleton<IRedisCacheService, RedisCacheService>();
			services.AddFluentValidationAutoValidation()
		.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
			services.AddScoped<ValidationFilter>();
			services.AddIdentity<AppUser, AppRole>().AddRoles<AppRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
			var assembly = Assembly.GetExecutingAssembly();
			services.AddAutoMapper(assembly);
			services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(opt =>
			{
				opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidIssuer = configuration["Jwt:Issuer"],
					ValidateAudience = true,
					ValidAudience = configuration["Jwt:Audience"],
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"])),
					LifetimeValidator=(notBefore,expires,securityToken,validationParameters)=>expires!=null ? expires>DateTime.UtcNow:false

				};
			});
			services.AddHttpContextAccessor();
			services.AddAuthorization(opt =>
			{
				opt.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));

			});
		}
	}
}
