using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Extensions
{
	public static class DalExtension
	{
		public static void AddDalExtension(this IServiceCollection services,IConfiguration configuration)
		{
			services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
			services.AddDbContext<AppDbContext>(opt =>
			{
				opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});
			services.AddScoped<IUnitOfWork, UnitOfWork>();
		}
	}
}
