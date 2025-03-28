using AutoMapper;
using DataAccessLayer.UnitOfWorks;
using EntityLayer.DTOs.PlaylistDtos;
using EntityLayer.DTOs.SongDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Concrete
{
	public class PlaylistService : IPlaylistService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<AppUser> _userManager;
		private readonly IMapper _mapper;
		private readonly IHttpContextAccessor _contextAccessor;

		public PlaylistService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper, IHttpContextAccessor contextAccessor)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_mapper = mapper;
			_contextAccessor = contextAccessor;
		}

		public async Task<List<PlaylistDto>> GetAllPlaylists()
		{
			var currentUser = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
			if (currentUser == null)
				throw new Exception("İstifadəçi tapılmadı");

			List<Playlist> playlists = await _unitOfWork.GetRepository<Playlist>()
				.GetAllAsync(x => !x.IsDeleted && x.UserId == currentUser.Id);

			if (playlists == null )
				throw new Exception("Heç bir playlist mövcud deyil");

			List<PlaylistDto> result = new List<PlaylistDto>();
			foreach (var playlist in playlists)
				result.Add(_mapper.Map<PlaylistDto>(playlist));
			return result;
		}
		public async Task RemovePlaylist(Guid id)
		{
			var currentUser = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
			var playlist = await _unitOfWork.GetRepository<Playlist>()
				.GetAsync(x => !x.IsDeleted && x.Id == id && x.UserId == currentUser.Id);
			if (playlist == null)
				throw new Exception("Playlist tapılmadı");
			playlist.IsDeleted = true;
			playlist.DeletedDate = DateTime.UtcNow;
			await _unitOfWork.GetRepository<Playlist>().UpdateAsync(playlist);
			await _unitOfWork.SaveAsynsc();
		}

		public async Task CreatePlaylist(string name, ClaimsPrincipal user)
		{
			var activeUser = await _userManager.GetUserAsync(user);
			if (activeUser == null)
				throw new Exception("İstifadəçi tapılmadı");

			bool existingPlaylist = await _unitOfWork.GetRepository<Playlist>()
				.AnyAsync(p => !p.IsDeleted && p.UserId == activeUser.Id && p.Name.ToLower() == name.ToLower());

			if (existingPlaylist)
				throw new Exception("Bu adda playlist artıq mövcuddur!");

			Playlist playlist = new Playlist
			{
				Name = name,
				UserId = activeUser.Id
			};

			await _unitOfWork.GetRepository<Playlist>().AddAsync(playlist);
			await _unitOfWork.SaveAsynsc();
		}

		public async Task<List<SongDto>> AddSongToPlaylist(Guid playlistId, Guid songId)
		{

			Song song = await _unitOfWork.GetRepository<Song>()
				.GetAsync(x => !x.IsDeleted && x.Id == songId);
			if (song == null)
				throw new Exception("Mahnı tapılmadı");


			Playlist songPlaylist = await _unitOfWork.GetRepository<Playlist>()
				.GetAsync(x => !x.IsDeleted && x.Id == playlistId,
						  include: query => query
							  .Include(p => p.Songs.Where(x => !x.IsDeleted))
							  .ThenInclude(s => s.Artists));
			if (songPlaylist == null)
				throw new Exception("Playlist tapılmadı");


			if (songPlaylist.Songs.Any(x => x.Id == songId))
				throw new Exception("Bu mahnı artıq playlist-də mövcuddur");


			songPlaylist.Songs.Add(song);
			songPlaylist.ReCalculation();
			songPlaylist.UpdatedDate = DateTime.Now;
			await _unitOfWork.SaveAsynsc();


			List<SongDto> songDtos = new List<SongDto>();
			foreach (var dto in songPlaylist.Songs)
			{
				var map = _mapper.Map<SongDto>(dto);

				map.ArtistNames = dto.Artists.Select(a => a.FullName).ToList();
				songDtos.Add(map);
			}
			return songDtos;
		}
		public async Task<List<SongDto>> RemoveSongFromPlaylist(Guid playlistId, Guid songId)
		{

			Song song = await _unitOfWork.GetRepository<Song>()
				.GetAsync(x => !x.IsDeleted && x.Id == songId);
			if (song == null)
				throw new Exception("Mahnı tapılmadı");


			Playlist songPlaylist = await _unitOfWork.GetRepository<Playlist>()
				.GetAsync(x => !x.IsDeleted && x.Id == playlistId,
						  include: query => query
							  .Include(p => p.Songs.Where(x => !x.IsDeleted))
							  .ThenInclude(s => s.Artists));
			if (songPlaylist == null)
				throw new Exception("Playlist tapılmadı");



			songPlaylist.Songs.Remove(song);
			songPlaylist.ReCalculation();
			songPlaylist.UpdatedDate = DateTime.Now;
			await _unitOfWork.SaveAsynsc();

			List<SongDto> songDtos = new List<SongDto>();
			foreach (var dto in songPlaylist.Songs)
			{
				var map = _mapper.Map<SongDto>(dto);
				map.ArtistNames = dto.Artists.Select(a => a.FullName).ToList();
				songDtos.Add(map);
			}
			return songDtos;
		}

		public async Task<List<SongDto>> GetAllSongsInPlaylist(Guid playlistId)
		{
			Playlist songPlaylist = await _unitOfWork.GetRepository<Playlist>()
				.GetAsync(x => !x.IsDeleted && x.Id == playlistId,
						  include: query => query
							  .Include(p => p.Songs)
							  .ThenInclude(s => s.Artists));
			if (songPlaylist == null)
				throw new Exception("Playlist tapılmadı");

			List<SongDto> songDtos = new List<SongDto>();
			foreach (var dto in songPlaylist.Songs)
			{
				if (!dto.IsDeleted)
				{
					var map = _mapper.Map<SongDto>(dto);
					map.ArtistNames = dto.Artists.Select(a => a.FullName).ToList();
					songDtos.Add(map);
				}
			}
			return songDtos;
		}
		public async Task<PlaylistDto> UpdatePlaylistAsync(UpdatePlaylistDto updatePlaylistDto,ClaimsPrincipal user)
		{
			var currentUser=await _userManager.GetUserAsync(user);
			var playlist = await _unitOfWork.GetRepository<Playlist>().GetAsync(x => !x.IsDeleted && x.Id == updatePlaylistDto.Id && x.User.Id==currentUser.Id,
				include: query => query.Include(p => p.Songs
				.Where(x => !x.IsDeleted))
				.ThenInclude(x => x.Artists));
			if (playlist == null) throw new Exception("Playlist tapilmadi");
			_mapper.Map(updatePlaylistDto, playlist);
			playlist.UpdatedDate = DateTime.Now;
			await _unitOfWork.GetRepository<Playlist>().UpdateAsync(playlist);
			await _unitOfWork.SaveAsynsc();
			var map = _mapper.Map<PlaylistDto>(playlist);
			return map;
		}
		public async Task<List<PlaylistDto>> GetPlaylistsByName(string name)
		{
			var playlists = await _unitOfWork.GetRepository<Playlist>()
				.GetAllAsync(x => !x.IsDeleted && x.Name.ToLower().Contains(name.ToLower()));
			if (playlists == null || playlists.Count == 0)
				throw new Exception("Playlist tapılmadı");

			List<PlaylistDto> dtos = new List<PlaylistDto>();
			foreach (var playlist in playlists)
				dtos.Add(_mapper.Map<PlaylistDto>(playlist));
			return dtos;
		}
	}
}
