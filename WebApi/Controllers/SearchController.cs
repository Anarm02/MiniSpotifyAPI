using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.Abstract;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SearchController : ControllerBase
	{
		private readonly ISearchService _searchService;

		public SearchController(ISearchService searchService)
		{
			_searchService = searchService;
		}


		[HttpGet]
		public async Task<IActionResult> Search([FromQuery] string query)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return BadRequest("Axtarış üçün sorğu daxil edin.");
			}

			
				var result = await _searchService.SearchAsync(query);
				return Ok(result);
			
		}
	}
}
