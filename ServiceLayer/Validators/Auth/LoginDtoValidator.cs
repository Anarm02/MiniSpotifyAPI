﻿using EntityLayer.DTOs.AuthDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validators.Auth
{
	public class LoginDtoValidator : AbstractValidator<LoginDto>
	{
		public LoginDtoValidator()
		{
			RuleFor(x=>x.Email).NotEmpty().EmailAddress();
			RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
		}
	}
}
