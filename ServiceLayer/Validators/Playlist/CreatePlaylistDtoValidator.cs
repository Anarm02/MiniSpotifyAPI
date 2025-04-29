using EntityLayer.DTOs.PlaylistDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validators.Playlist
{
	public class CreatePlaylistDtoValidator : AbstractValidator<CreatePlaylistDto>
	{
		public CreatePlaylistDtoValidator()
		{
			RuleFor(x=>x.Name).NotEmpty().MinimumLength(3);
		}
	}
}
