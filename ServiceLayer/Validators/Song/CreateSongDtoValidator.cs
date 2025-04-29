using EntityLayer.DTOs.SongDtos;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validators.Song
{
	public class CreateSongDtoValidator : AbstractValidator<CreateSongDto>
	{
		private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
		private const long _fileSizeLimit = 5 * 1024 * 1024; 
		public CreateSongDtoValidator()
		{
			RuleFor(x=>x.Title).NotEmpty().MinimumLength(3);
			RuleFor(x => x.MusicFile)
			.NotNull().WithMessage("Fayl seçilməlidir.")
			.Must(BeAValidMusicFile).WithMessage("Yalnız musiqi faylı yükləyə bilərsiniz. Dəstəklənən formatlar: .mp3, .wav, .flac.");
			RuleFor(x => x.ArtworkFile)
				.Must(BeAValidImage).When(x => x.ArtworkFile != null)
				.WithMessage($"Artwork file yalnızca {string.Join(", ", _permittedExtensions)} uzantılı ve {_fileSizeLimit / (1024 * 1024)} MB'den küçük olmalıdır.");
		}
		private bool BeAValidMusicFile(IFormFile file)
		{
			if (file == null)
				return false;

			
			var allowedMimeTypes = new[] { "audio/mpeg", "audio/wav", "audio/x-wav", "audio/flac" };
			var fileMimeType = file.ContentType.ToLower();

			if (!allowedMimeTypes.Contains(fileMimeType))
				return false;

			
			var allowedExtensions = new[] { ".mp3", ".wav", ".flac" };
			var fileExtension = System.IO.Path.GetExtension(file.FileName)?.ToLower();

			return allowedExtensions.Contains(fileExtension);
		}
		private bool BeAValidImage(IFormFile file)
		{
			if (file.Length <= 0 || file.Length > _fileSizeLimit)
				return false;

			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
			if (string.IsNullOrEmpty(extension) || !_permittedExtensions.Contains(extension))
				return false;

			if (!file.ContentType.StartsWith("image/"))
				return false;

			return true;
		}
	}
}
