using EntityLayer.DTOs.SongDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Abstract
{
	public interface ISongService
	{
		Task<string> SaveFileAsync(IFormFile file);
		Task<List<SongDto>> GetAllSongsAsync();
		Task<SongDto> GetSongByIdAsync(Guid id);
		Task<List<SongDto>> GetSongByNameAsync(string name);
		Task<SongDto> UploadSongAsync(IFormFile file, string title, ClaimsPrincipal user, IEnumerable<string> additionalArtistNames = null);
		Task<List<SongDto>> GetSongByArtist(Guid artistId);
		Task<List<SongDto>> GetSongsByArtistName(string artistName);
		Task RemoveSong(Guid id,ClaimsPrincipal user);
	}
}
