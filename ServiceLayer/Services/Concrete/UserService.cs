using AutoMapper;
using DataAccessLayer.UnitOfWorks;
using EntityLayer.DTOs.SongDtos;
using EntityLayer.DTOs.UserDtos;
using EntityLayer.Entities;
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
		private readonly IMapper mapper;
		private readonly IUnitOfWork _unitOfWork;
		public UserService(UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor, IMapper mapper, IUnitOfWork unitOfWork)
		{
			_userManager = userManager;
			_contextAccessor = contextAccessor;
			this.mapper = mapper;
			_unitOfWork = unitOfWork;
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
			// Cari istifadəçini al və admin yoxlamasını et.
			var currentUser = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
			if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
			{
				throw new UnauthorizedAccessException("Rol dəyişikliyi etmək üçün admin hüququna malik deyilsiniz.");
			}

			// Silinəcək istifadəçini tap.
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				throw new Exception("Istifadeci tapilmadi");

			// İstifadəçiyə aid mahnıları tap və sil.
			var userSongs = await _unitOfWork.GetRepository<Song>()
	.GetAllAsync(x => x.Artists.Any(a => a.Id.ToString() == userId));

			if (userSongs != null && userSongs.Any())
			{
				foreach (var song in userSongs)
				{
				await	_unitOfWork.GetRepository<Song>().DeleteAsync(song);
				}
			}

			// İstifadəçiyə aid playlistləri tap və sil.
			var userPlaylists = await _unitOfWork.GetRepository<Playlist>().GetAllAsync(x=>x.UserId.ToString()==userId);
			if (userPlaylists != null && userPlaylists.Any())
			{
				foreach (var playlist in userPlaylists)
				{
				await	_unitOfWork.GetRepository<Playlist>().DeleteAsync(playlist);
				}
			}

			// İstifadəçini sil və bütün əməliyyatları yaddaşa yaz.
			await _userManager.DeleteAsync(user);
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

		
	}
}
