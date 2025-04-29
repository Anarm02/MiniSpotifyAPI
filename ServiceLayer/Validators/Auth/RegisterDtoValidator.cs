using EntityLayer.DTOs.AuthDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validators.Auth
{
	public class RegisterDtoValidator : AbstractValidator<RegisterDto>
	{
		public RegisterDtoValidator()
		{
			RuleFor(x=>x.Email).NotEmpty().EmailAddress().MinimumLength(12);
			RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
			RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password);
			RuleFor(x => x.FullName).NotEmpty().MinimumLength(2);
		}
	}
}
