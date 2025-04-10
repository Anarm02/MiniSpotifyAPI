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
		public async Task<IActionResult> CreatePlaylist(string name)
		{
			try
			{
				await _playlistService.CreatePlaylist(name, User);
				return Ok("Playlist yaradildi");
			}
			catch (Exception ex)
			{

				return BadRequest(ex.Message);
			}
		}
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetAllPlaylists()
		{
			try
			{
				var playlists = await _playlistService.GetAllPlaylists();
				return Ok(playlists);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPost("{playlistId}/{songId}")]
		public async Task<IActionResult> AddSongToPlaylist(Guid playlistId, Guid songId)
		{
			try
			{
				var playlist = await _playlistService.AddSongToPlaylist(playlistId, songId);
				return Ok(playlist);
			}
			catch (Exception ex)
			{

				return BadRequest(ex.Message);
			}
		}
		[Authorize]
		[HttpPut]
		public async Task<IActionResult> UpdatePlaylist(UpdatePlaylistDto playlistDto)
		{
			try
			{
				var playlist = await _playlistService.UpdatePlaylistAsync(playlistDto, User);
				return Ok(playlist);
			}
			catch (Exception ex)
			{

				return BadRequest(ex.Message);
			}
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetSongsInPlaylist(Guid id)
		{
			try
			{
				var songs = await _playlistService.GetAllSongsInPlaylist(id);
				return Ok(songs);
			}
			catch (Exception ex)
			{

				return BadRequest(ex.Message);
			}
		}
		[Authorize]
		[HttpDelete("{id}")]
		public async Task<IActionResult> RemovePlaylist(Guid id)
		{
			try
			{
				await _playlistService.RemovePlaylist(id);
				return Ok("Playlist silindi");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet("[action]")]
		public async Task<IActionResult> SearchByName(string name)
		{
			try
			{
				var playlists = await _playlistService.GetPlaylistsByName(name);
				return Ok(playlists);
			}
			catch (Exception ex)
			{

				return BadRequest(ex.Message);
			}
		}
		[HttpDelete("{playlistId}/{songId}")]
		public async Task<IActionResult> RemoveSongFromPlaylist(Guid playlistId, Guid songId)
		{
			try
			{
				var songs = await _playlistService.RemoveSongFromPlaylist(playlistId, songId);
				return Ok(songs);
			}
			catch (Exception ex)
			{

				return BadRequest(ex.Message);
			}
		}
	}
}
