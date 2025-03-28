using EntityLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
	public interface IRepository<T> where T : class, IEntityBase
	{
		Task AddAsync(T entity);
		Task<List<T>> GetAllAsync(
	Expression<Func<T, bool>> predicate = null,
	Func<IQueryable<T>, IQueryable<T>> include = null);
		Task<T> GetAsync(
			Expression<Func<T, bool>> predicate,
			Func<IQueryable<T>, IQueryable<T>> include = null);
		Task<T> GetByGuidAsync(Guid id);
		Task<T> UpdateAsync(T entity);
		Task DeleteAsync(T entity);
		Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
		Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
	}
}
