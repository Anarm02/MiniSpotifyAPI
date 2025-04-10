using EntityLayer.DTOs.SearchDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Abstract
{
	public interface ISearchService
	{
		Task<SearchResultDto> SearchAsync(string query);
	}
}
