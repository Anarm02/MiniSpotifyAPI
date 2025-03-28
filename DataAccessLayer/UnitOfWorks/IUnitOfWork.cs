using DataAccessLayer.Repositories;
using EntityLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.UnitOfWorks
{
	public interface IUnitOfWork
	{
		IRepository<T> GetRepository<T>() where T : class, IEntityBase, new();
		Task<int> SaveAsynsc();
		int Save();
	}
}
