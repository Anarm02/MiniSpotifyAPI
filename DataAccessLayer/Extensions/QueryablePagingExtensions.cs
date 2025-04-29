using AutoMapper;
using DataAccessLayer.Helpers;
using Microsoft.EntityFrameworkCore;

public static class QueryablePagingExtensions
{
	public static async Task<PagedResult<TDest>> ToPagedResultAsync<TSource, TDest>(
		this IQueryable<TSource> query,
		IMapper mapper,
		int page,
		int pageSize
	)
	{
		if (page <= 0) page = 1;
		if (pageSize <= 0) pageSize = 10;

		var total = await query.CountAsync();
		var items = await query
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		// AutoMapper ile TSource -> TDest çevirmək
		var dtos = mapper.Map<List<TDest>>(items);

		return new PagedResult<TDest>
		{
			Items = dtos,
			TotalCount = total,
			Page = page,
			PageSize = pageSize
		};
	}
}
