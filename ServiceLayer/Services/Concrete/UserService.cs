using AutoMapper;
using DataAccessLayer.UnitOfWorks;
using EntityLayer.DTOs.SongDtos;
using EntityLayer.DTOs.UserDtos;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Concrete
{
	public class UserService : IUserService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly RoleManager<AppRole> _roleManager;
		private readonly IMapper mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public UserService(UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor, IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, RoleManager<AppRole> roleManager)
		{
			_userManager = userManager;
			_contextAccessor = contextAccessor;
			this.mapper = mapper;
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
			_roleManager = roleManager;
		}

		public async Task<List<UserDto>> GetAllUsers()
		{
			
			var currentUser = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
			var users = await _userManager.Users
							.Where(u => u.Id != currentUser.Id)
							.ToListAsync();

			var userDtos = new List<UserDto>();
			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);
				userDtos.Add(new UserDto
				{
					Id = user.Id,
					FullName = user.FullName,
					Email = user.Email,
					UserName = user.UserName,
					PhoneNumber = user.PhoneNumber,
					Roles = roles 
				});
			}

			return userDtos;
		}


		public async Task<UserDto> EditUser(EditUserDto editUser)
		{
			var currentUser = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
			mapper.Map(editUser, currentUser);
			var map = mapper.Map<UserDto>(currentUser);
			return map;
		}
		public async Task RemoveUser(string userId)
		{
			
			var currentUser = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				throw new UnauthorizedAccessException("Rol dəyişikliyi etmək üçün admin hüququna malik deyilsiniz.");
			}

			
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				throw new Exception("İstifadəçi tapılmadı");

			
			var userSongs = await _unitOfWork.GetRepository<Song>()
				.GetAllAsync(
					x => x.Artists.Any(a => a.Id.ToString() == userId),
					include: query => query.Include(x => x.Playlists)
				);

			if (userSongs != null && userSongs.Any())
			{
				foreach (var song in userSongs)
				{
					
					if (!string.IsNullOrEmpty(song.FilePath))
					{
						var relativePath = song.FilePath.TrimStart('/');
						var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
						if (File.Exists(absolutePath))
						{
							File.Delete(absolutePath);
						}
					}

					
					foreach (var playlist in song.Playlists.ToList())
					{
						playlist.Songs.Remove(song);
						await _unitOfWork.GetRepository<Playlist>().UpdateAsync(playlist);
					}

					
					await _unitOfWork.GetRepository<Song>().DeleteAsync(song);
				}
			}

			
			var userPlaylists = await _unitOfWork.GetRepository<Playlist>()
				.GetAllAsync(
					x => x.UserId.ToString() == userId,
					include: query => query.Include(p => p.Songs)
				);

			if (userPlaylists != null && userPlaylists.Any())
			{
				foreach (var playlist in userPlaylists)
				{
				
					foreach (var song in playlist.Songs.ToList())
					{
						song.Playlists.Remove(playlist);
						await _unitOfWork.GetRepository<Song>().UpdateAsync(song);
					}

					
					await _unitOfWork.GetRepository<Playlist>().DeleteAsync(playlist);
				}
			}

			
			await _unitOfWork.SaveAsynsc();

			var identityResult = await _userManager.DeleteAsync(user);
			if (!identityResult.Succeeded)
			{
				throw new Exception("İstifadəçi silinərkən xəta baş verdi.");
			}

			await _unitOfWork.SaveAsynsc();
		}






		public async Task<bool> ChangeUserRolesAsync(string userId, List<string> newRoles)
		{	
			var currentUser = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				throw new UnauthorizedAccessException("Rol dəyişikliyi etmək üçün admin hüququna malik deyilsiniz.");
			}
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				throw new Exception("İstifadəçi tapılmadı.");
			}
			var currentRoles = await _userManager.GetRolesAsync(user);
			var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
			if (!removeResult.Succeeded)
			{
				throw new Exception("Mövcud rolların silinməsi zamanı xəta baş verdi.");
			}
			var addResult = await _userManager.AddToRolesAsync(user, newRoles);
			if (!addResult.Succeeded)
			{
				throw new Exception("Yeni rolların təyin olunması zamanı xəta baş verdi.");
			}
			return true;
		}

		public async Task<List<string>> GetAllRolesAsync()
		{
			var roles = await _roleManager.Roles.Select(r=>r.Name).ToListAsync();
			if (roles == null)
				throw new Exception("Rollar tapilmadi");
			return roles;
		}

		
	}
}
