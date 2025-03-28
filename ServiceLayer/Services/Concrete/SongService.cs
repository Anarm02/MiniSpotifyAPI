using AutoMapper;
using DataAccessLayer.UnitOfWorks;
using EntityLayer.DTOs.SongDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Services.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TagLib;
using File = System.IO.File;

namespace ServiceLayer.Services.Concrete
{
	public class SongService : ISongService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly UserManager<AppUser> _userManager;

		public SongService(IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
		{
			_webHostEnvironment = webHostEnvironment;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_userManager = userManager;
		}

		public async Task<List<SongDto>> GetAllSongsAsync()
		{
			var songs = await _unitOfWork.GetRepository<Song>()
				.GetAllAsync(x => !x.IsDeleted, include: q => q.Include(x => x.Artists));

			List<SongDto> songDtos = new List<SongDto>();
			foreach (var song in songs)
			{
				var map = _mapper.Map<SongDto>(song);
				map.ArtistNames = song.Artists.Select(a => a.FullName).ToList();
				songDtos.Add(map);
			}
			return songDtos;
		}

		public async Task<SongDto> GetSongByIdAsync(Guid id)
		{
			var song = await _unitOfWork.GetRepository<Song>()
				.GetAsync(x => !x.IsDeleted && x.Id == id, include: q => q.Include(x => x.Artists));
			var map = _mapper.Map<SongDto>(song);
			map.ArtistNames = song.Artists.Select(a => a.FullName).ToList();
			return map;
		}

		public async Task<List<SongDto>> GetSongByArtist(Guid artistId)
		{

			var songs = await _unitOfWork.GetRepository<Song>()
				.GetAllAsync(x => !x.IsDeleted && x.Artists.Any(a => a.Id == artistId),
							 include: q => q.Include(x => x.Artists));

			List<SongDto> songDtos = new List<SongDto>();
			foreach (var song in songs)
			{
				var map = _mapper.Map<SongDto>(song);
				map.ArtistNames = song.Artists.Select(a => a.FullName).ToList();
				songDtos.Add(map);
			}
			return songDtos;
		}

		public async Task<string> SaveFileAsync(IFormFile file)
		{
			var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "songs");

			if (!Directory.Exists(uploadsFolder))
			{
				Directory.CreateDirectory(uploadsFolder);
			}

			var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
			var filePath = Path.Combine(uploadsFolder, uniqueFileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			return $"/uploads/songs/{uniqueFileName}";
		}

		public async Task<SongDto> UploadSongAsync(IFormFile file, string title, ClaimsPrincipal user, IEnumerable<string> additionalArtistNames = null)
		{
			if (string.IsNullOrWhiteSpace(title))
				throw new Exception("Mahniya ad verilmeyib");

			var filePath = await SaveFileAsync(file);
			var duration = GetMp3Duration(filePath);
			var uploader = await _userManager.GetUserAsync(user);
			if (uploader == null)
				throw new UnauthorizedAccessException("Ifaçı tapılmadı");

			var artists = new List<AppUser> { uploader };

			if (additionalArtistNames != null)
			{
				foreach (var artistName in additionalArtistNames)
				{
					if (!string.IsNullOrWhiteSpace(artistName))
					{
						var existingArtist = await _userManager.Users.FirstOrDefaultAsync(a => a.FullName == artistName);
						if (existingArtist != null &&
							await _userManager.IsInRoleAsync(existingArtist, "Artist") &&
							!artists.Any(a => a.Id == existingArtist.Id))
						{
							artists.Add(existingArtist);
						}
						else
						{
							throw new Exception($"{artistName} ifaci deyil.");
						}
					}
				}
			}

				Song song = new Song
				{
					Title = title,
					FilePath = filePath,
					Duration = duration,
					Artists = artists
				};

				await _unitOfWork.GetRepository<Song>().AddAsync(song);
				await _unitOfWork.SaveAsynsc();

				var map = _mapper.Map<SongDto>(song);
				map.ArtistNames = song.Artists.Select(a => a.FullName).ToList();
				return map;
			
		}


		private TimeSpan GetMp3Duration(string filePath)
		{
			var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
			using var file = TagLib.File.Create(fullPath);
			return file.Properties.Duration;
		}

		public async Task<List<SongDto>> GetSongsByArtistName(string artistName)
		{
			var artist = await _userManager.Users.FirstOrDefaultAsync(x => x.FullName == artistName);
			if (artist == null)
				return new List<SongDto>();

			var songs = await _unitOfWork.GetRepository<Song>()
				.GetAllAsync(x => !x.IsDeleted && x.Artists.Any(a => a.Id == artist.Id),
							 include: q => q.Include(x => x.Artists));

			List<SongDto> songDtos = new List<SongDto>();
			foreach (var song in songs)
			{
				var map = _mapper.Map<SongDto>(song);
				map.ArtistNames = song.Artists.Select(a => a.FullName).ToList();
				songDtos.Add(map);
			}
			return songDtos;
		}

		public async Task<List<SongDto>> GetSongByNameAsync(string name)
		{
			var songs = await _unitOfWork.GetRepository<Song>().GetAllAsync(
				x => !x.IsDeleted && x.Title.ToLower().Contains(name.ToLower()),
				include: q => q.Include(x => x.Artists));

			if (songs == null || songs.Count == 0)
				throw new Exception("Bu adda mahnı yoxdu");

			List<SongDto> dtos = new List<SongDto>();
			foreach (var song in songs)
			{
				var map = _mapper.Map<SongDto>(song);
				map.ArtistNames = song.Artists.Select(a => a.FullName).ToList();
				dtos.Add(map);
			}

			return dtos;
		}
		public async Task<SongDto> UpdateSongAsync(UpdateSongDto updateSongDto,ClaimsPrincipal user)
		{
			var currentUser = await _userManager.GetUserAsync(user);
			if (currentUser == null) throw new Exception("Istifadeci tapilmadi");
			var song = await _unitOfWork.GetRepository<Song>().GetAsync(x => !x.IsDeleted && x.Id == updateSongDto.Id && x.Artists.Any(x => x.Id == currentUser.Id),
				include: x => x.Include(s => s.Artists));
			if (song == null) throw new Exception("Mahni tapilmadi");
			song.Title = updateSongDto.Name;
			song.UpdatedDate= DateTime.Now;
			await _unitOfWork.GetRepository<Song>().UpdateAsync(song);
			await _unitOfWork.SaveAsynsc();
			var map= _mapper.Map<SongDto>(song);
			return map;
		}
		public async Task RemoveSong(Guid id, ClaimsPrincipal user)
		{
			var song = await _unitOfWork.GetRepository<Song>()
				.GetAsync(
					x => !x.IsDeleted && x.Id == id,
					include: q => q.Include(x => x.Artists)
								  .Include(x => x.Playlists)
								  .ThenInclude(p => p.Songs)
				);
			if (song == null)
				throw new Exception("Mahni tapilmadi");

			var currentUser = await _userManager.GetUserAsync(user);

			if (!song.Artists.Any(a => a.Id == currentUser.Id) && !user.IsInRole("Admin"))
				throw new Exception("Yalniz oz mahninizi sile bilersiniz");

			if (!string.IsNullOrEmpty(song.FilePath))
			{
				var relativePath = song.FilePath.TrimStart('/');
				var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
				if (File.Exists(absolutePath))
				{
					File.Delete(absolutePath);
					song.IsDeleted = true;
					song.DeletedDate = DateTime.UtcNow;
					await _unitOfWork.GetRepository<Song>().UpdateAsync(song);
					foreach (var playlist in song.Playlists)
					{
						playlist.Duration = new TimeSpan(
							playlist.Songs.Where(s => !s.IsDeleted)
										  .Sum(s => s.Duration.Ticks)
						);
						await _unitOfWork.GetRepository<Playlist>().UpdateAsync(playlist);
					}

					await _unitOfWork.SaveAsynsc();
				}
				else
				{
					throw new Exception($"Mahni siline bilmedi. Fayl tapılmadı: {absolutePath}");
				}
			}
			else
			{
				throw new Exception("Mahnının fayl yolu qeyd edilməyib");
			}
		}

	}
}
