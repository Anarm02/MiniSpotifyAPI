using DataAccessLayer.UnitOfWorks;
using EntityLayer.DTOs.PlaylistDtos;
using EntityLayer.DTOs.SongDtos;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Abstract
{
	public interface IPlaylistService
	{
		Task CreatePlaylist(string name,ClaimsPrincipal user);
		Task<List<SongDto>> AddSongToPlaylist(Guid playlistId, Guid songId);
		Task<List<SongDto>> RemoveSongFromPlaylist(Guid playlistId, Guid songId);
		Task<List<SongDto>> GetAllSongsInPlaylist(Guid playlistId);
		Task<List<PlaylistDto>> GetAllPlaylists();
		Task RemovePlaylist(Guid id);
		Task<List<PlaylistDto>> GetPlaylistsByName(string name);
		Task<PlaylistDto> UpdatePlaylistAsync(UpdatePlaylistDto updatePlaylistDto,ClaimsPrincipal user);
	}
}
