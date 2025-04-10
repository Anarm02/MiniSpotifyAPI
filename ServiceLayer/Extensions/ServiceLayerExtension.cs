using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Services.Abstract;
using ServiceLayer.Services.Concrete;
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

		
		public static void AddServiceLayer(this IServiceCollection services,IConfiguration configuration)
		{
			services.AddScoped<ITokenService, TokenService>();	
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<ISongService,SongService>();
			services.AddScoped<IPlaylistService, PlaylistService>();
			services.AddScoped<ISearchService, SearchService>();
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
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]))
				
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
