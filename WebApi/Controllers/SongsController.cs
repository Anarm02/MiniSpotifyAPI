using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.Abstract;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SongsController : ControllerBase
	{
		private readonly ISongService _songService;

		public SongsController(ISongService songService)
		{
			_songService = songService;
		}
		[HttpPost("upload")]
		[Authorize(Roles = "Artist")]
		public async Task<IActionResult> Upload(IFormFile file, string title, [FromForm] List<string>? additionalArtistNames)
		{
			if (file == null || file.Length == 0)
				return BadRequest("Fayl seçilməyib.");

			try
			{
				
				var song = await _songService.UploadSongAsync(file,title,User,additionalArtistNames);
				return Ok(song);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[Authorize(Roles ="Admin,Artist")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> RemoveSongs(Guid id)
		{
			try
			{
				await _songService.RemoveSong(id,User);
				return Ok("Mahni silindi");
			}
			catch(Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet]
		public async Task<IActionResult> GetAllSongs()
		{
			var songs = await _songService.GetAllSongsAsync();
			return Ok(songs);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetSong(Guid id)
		{
			var song = await _songService.GetSongByIdAsync(id);
			if (song == null)
				return BadRequest("Mahnı tapılmadı");
			return Ok(song);
		}
		[HttpGet("[action]/{id}")]
		public async Task<IActionResult> SongByArtist(Guid id)
		{
			var songs = await _songService.GetSongByArtist(id);
			if (songs == null || songs.Count == 0)
				return BadRequest("Bu ifaçının mahnısı yoxdu");
			return Ok(songs);
		}
		

	}
}
