using EntityLayer.DTOs.PlaylistDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.Abstract;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlaylistController : ControllerBase
	{
		private readonly IPlaylistService _playlistService;

		public PlaylistController(IPlaylistService playlistService)
		{
			_playlistService = playlistService;
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreatePlaylist(CreatePlaylistDto createPlaylistDto)
		{
			
				await _playlistService.CreatePlaylist(createPlaylistDto, User);
				return Ok("Playlist yaradildi");
			

				

		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetAllPlaylists()
		{
			
				var playlists = await _playlistService.GetAllPlaylists();
				return Ok(playlists);
			
		}

		[HttpPost("{playlistId}/{songId}")]
		public async Task<IActionResult> AddSongToPlaylist(Guid playlistId, Guid songId)
		{
			
				var playlist = await _playlistService.AddSongToPlaylist(playlistId, songId);
				return Ok(playlist);
			
		}

		[Authorize]
		[HttpPut]
		public async Task<IActionResult> UpdatePlaylist(UpdatePlaylistDto playlistDto)
		{
			
				var playlist = await _playlistService.UpdatePlaylistAsync(playlistDto, User);
				return Ok(playlist);
			
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetSongsInPlaylist(Guid id)
		{
			
				var songs = await _playlistService.GetAllSongsInPlaylist(id);
				return Ok(songs);
			
		}

		[Authorize]
		[HttpDelete("{id}")]
		public async Task<IActionResult> RemovePlaylist(Guid id)
		{
			
				await _playlistService.RemovePlaylist(id);
				return Ok("Playlist silindi");
			
		}

		[HttpGet("[action]")]
		public async Task<IActionResult> SearchByName(string name)
		{
			
				var playlists = await _playlistService.GetPlaylistsByName(name);
				return Ok(playlists);
			
		}

		[HttpDelete("{playlistId}/{songId}")]
		public async Task<IActionResult> RemoveSongFromPlaylist(Guid playlistId, Guid songId)
		{
			
				var songs = await _playlistService.RemoveSongFromPlaylist(playlistId, songId);
				return Ok(songs);
			
		}
	}
}
