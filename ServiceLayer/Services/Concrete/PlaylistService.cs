using AutoMapper;
using DataAccessLayer.UnitOfWorks;
using EntityLayer.DTOs.PlaylistDtos;
using EntityLayer.DTOs.SongDtos;
using EntityLayer.Entities;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Exceptions;
using ServiceLayer.Helpers.ImageHelpers;
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
		private readonly IImageHelper _imageHelper;

		public PlaylistService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper, IHttpContextAccessor contextAccessor, IImageHelper imageHelper)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_mapper = mapper;
			_contextAccessor = contextAccessor;
			_imageHelper = imageHelper;
		}

		public async Task<List<PlaylistDto>> GetAllPlaylists()
		{
			var currentUser = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
			if (currentUser == null)
				throw new UserNotFoundException("İstifadəçi tapılmadı");

			List<Playlist> playlists = await _unitOfWork.GetRepository<Playlist>()
				.GetAllAsync(x => !x.IsDeleted && x.UserId == currentUser.Id,include:q=>q.Include(x=>x.CoverImage));

			if (playlists == null )
				throw new KeyNotFoundException("Heç bir playlist mövcud deyil");

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

		public async Task CreatePlaylist(CreatePlaylistDto createPlaylistDto, ClaimsPrincipal user)
		{
			var activeUser = await _userManager.GetUserAsync(user);
			if (activeUser == null)
				throw new UserNotFoundException("İstifadəçi tapılmadı");

			bool existingPlaylist = await _unitOfWork.GetRepository<Playlist>()
				.AnyAsync(p => !p.IsDeleted && p.UserId == activeUser.Id && p.Name.ToLower() == createPlaylistDto.Name.ToLower());

			if (existingPlaylist)
				throw new Exception("Bu adda playlist artıq mövcuddur!");

			Playlist playlist = new Playlist
			{
				Name = createPlaylistDto.Name,
				UserId = activeUser.Id
			};
			if (createPlaylistDto.ArtworkFile != null)
			{
				var image = await _imageHelper.SaveImageAsync(createPlaylistDto.ArtworkFile, ImageType.Playlist);
				playlist.CoverImageId = image.Id;
			}
			await _unitOfWork.GetRepository<Playlist>().AddAsync(playlist);
			await _unitOfWork.SaveAsynsc();
		}

		public async Task<List<SongDto>> AddSongToPlaylist(Guid playlistId, Guid songId)
		{
			
			var song = await _unitOfWork.GetRepository<Song>()
				.GetAsync(x => !x.IsDeleted && x.Id == songId);
			if (song == null)
				throw new Exception("Mahni tapılmadı");

			
			var songPlaylist = await _unitOfWork.GetRepository<Playlist>()
				.GetAsync(
					x => !x.IsDeleted && x.Id == playlistId,
					include: query => query
						.Include(p => p.Songs.Where(s => !s.IsDeleted))
							.ThenInclude(s => s.Artists)
						.Include(p => p.Songs.Where(s => !s.IsDeleted))
							.ThenInclude(s => s.ArtworkImage)
				);
			if (songPlaylist == null)
				throw new Exception("Playlist tapılmadı");

			
			if (songPlaylist.Songs.Any(x => x.Id == songId))
				throw new Exception("Bu mahni artiq playlist-de movcuddur");

			
			songPlaylist.Songs.Add(song);
			songPlaylist.ReCalculation();
			songPlaylist.UpdatedDate = DateTime.Now;
			await _unitOfWork.SaveAsynsc();

			
			var songDtos = new List<SongDto>();
			foreach (var s in songPlaylist.Songs)
			{
				if (s.IsDeleted) continue;

				var dto = _mapper.Map<SongDto>(s);
				dto.ArtistNames = s.Artists.Select(a => a.FullName).ToList();
				dto.ImagePath = s.ArtworkImage != null ? s.ArtworkImage.Url : null;
				songDtos.Add(dto);
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
			
			var songPlaylist = await _unitOfWork.GetRepository<Playlist>()
				.GetAsync(
					x => !x.IsDeleted && x.Id == playlistId,
					include: query => query
						.Include(p => p.Songs)
							.ThenInclude(s => s.Artists)
						.Include(p => p.Songs)
							.ThenInclude(s => s.ArtworkImage)
				);

			if (songPlaylist == null)
				throw new Exception("Playlist tapılmadı");

			var songDtos = new List<SongDto>();
			foreach (var song in songPlaylist.Songs)
			{
				if (song.IsDeleted)
					continue;

				
				var dto = _mapper.Map<SongDto>(song);
				dto.ImagePath = song?.ArtworkImage?.Url;
				dto.ArtistNames = song.Artists.Select(a => a.FullName).ToList();

				
				

				songDtos.Add(dto);
			}

			return songDtos;
		}


		

		public async Task<PlaylistDto> UpdatePlaylistAsync(UpdatePlaylistDto updateDto, ClaimsPrincipal user)
		{
			
			var currentUser = await _userManager.GetUserAsync(user)
							  ?? throw new UserNotFoundException("User not found");

			
			var playlist = await _unitOfWork.GetRepository<Playlist>()
				.GetAsync(
					predicate: p => !p.IsDeleted
								   && p.Id == updateDto.Id
								   && p.User.Id == currentUser.Id,
					include: q => q
						.Include(p => p.Songs.Where(s => !s.IsDeleted))
							.ThenInclude(s => s.Artists)
						.Include(p => p.CoverImage)   
				);

			if (playlist == null)
				throw new Exception("Playlist tapılmadı");

			
			_mapper.Map(updateDto, playlist);

			
			if (updateDto.ArtWorkFile != null)
			{
				
				if (playlist.CoverImageId.HasValue)
					await _imageHelper.DeleteImageAsync(playlist.CoverImageId.Value);

				var img = await _imageHelper.SaveImageAsync(updateDto.ArtWorkFile, ImageType.Playlist);
				playlist.CoverImageId = img.Id;
			}

			playlist.UpdatedDate = DateTime.UtcNow;

			
			await _unitOfWork.GetRepository<Playlist>().UpdateAsync(playlist);
			await _unitOfWork.SaveAsynsc();

			
			var resultDto = _mapper.Map<PlaylistDto>(playlist);
			resultDto.ImagePath = playlist.CoverImage?.Url;

			return resultDto;
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
