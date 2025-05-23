﻿using EntityLayer.DTOs.AuthDtos;
using EntityLayer.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ServiceLayer.Exceptions;
using ServiceLayer.Services.Abstract;
using ServiceLayer.Validators.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Concrete
{

	public class AuthService : IAuthService
	{
		private readonly UserManager<AppUser> userManager;
		private readonly IConfiguration configuration;
		private readonly SignInManager<AppUser> signInManager;
		private readonly ITokenService tokenService;
		private readonly RoleManager<AppRole> roleManager;
		public AuthService(UserManager<AppUser> userManager, IConfiguration configuration, SignInManager<AppUser> signInManager, ITokenService tokenService, RoleManager<AppRole> roleManager)
		{
			this.userManager = userManager;
			this.configuration = configuration;
			this.signInManager = signInManager;
			this.tokenService = tokenService;
			this.roleManager = roleManager;
			
		}

		public async Task<string> Register(RegisterDto model)
		{
		
			var existingUser = await userManager.FindByEmailAsync(model.Email);
			if (existingUser != null)
			{
				throw new KeyNotFoundException("Bele istifadeci artiq var");
			}
			
			var user = new AppUser
			{
				FullName = model.FullName,
				UserName = model.Email,
				Email = model.Email,
				IsEmailVerified = false
			};

			var result = await userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
			{
				return string.Join(", ", result.Errors.Select(e => e.Description));
			}
			await userManager.AddToRoleAsync(user, "User");
			var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
			var roles = await userManager.GetRolesAsync(user);
			var jwtToken = tokenService.GenerateToken(user, roles);
			user.RefreshToken = jwtToken.RefreshToken;
			user.RefreshTokenExpiration = jwtToken.RefreshTokenExpirationDate;
			await userManager.UpdateAsync(user);
			var verificationLink = $"http://localhost:5157/api/auth/verify-email?userId={user.Id}&token={WebUtility.UrlEncode(token)}";

			//await SendVerificationEmail(user.Email, verificationLink);

			return "Qeydiyyat tamamlandı.";
		}
		public async Task<string> VerifyEmailAsync(string userId, string token)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null) return "İstifadəçi tapılmadı.";

			var result = await userManager.ConfirmEmailAsync(user, token);
			if (!result.Succeeded) return "Email təsdiqlənmədi.";

			user.IsEmailVerified = true;
			await userManager.UpdateAsync(user);

			return "Email uğurla təsdiqləndi!";
		}
		public async Task<LoginResponseDto> LoginAsync(LoginDto model)
			{

			AppUser? user = await userManager.FindByEmailAsync(model.Email);
			if (user == null)
				throw new UserNotFoundException("User not found");
			if (!await userManager.CheckPasswordAsync(user, model.Password))
				throw new UnauthorizedAccessException("Password is wrong");
			//if ((bool)!user.IsEmailVerified)
			//	return "Email doğrulanmayıb!";
			var roles = await userManager.GetRolesAsync(user);
			var token = tokenService.GenerateToken(user, roles);
			string rolesString = string.Join(", ", roles);
			return new()
			{
				AccessToken = token.AccessToken,
				RefreshToken = token.RefreshToken,
				Roles = rolesString,
                UserName=user.FullName,
				UserId=user.Id.ToString(),
			};
		}
		private async Task SendVerificationEmail(string email, string verificationLink)
		{
			var smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential("mammadli.anar2002@gmail.com", "gyvuivcbdbxciohf"),
				EnableSsl = true,
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress("your-email@gmail.com"),
				Subject = "Email doğrulama",
				Body = $"Zəhmət olmasa emailinizi təsdiqləmək üçün bu linkə keçin: <a href='{verificationLink}'>Təsdiqlə</a>",
				IsBodyHtml = true,
			};

			mailMessage.To.Add(email);
			await smtpClient.SendMailAsync(mailMessage);

		}

		public async Task LogoutAsync()
		{
			await signInManager.SignOutAsync();
		}
	}
}
