using DataAccessLayer.UnitOfWorks;
using EntityLayer.Entities;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ServiceLayer.Helpers.ImageHelpers
{
	public class ImageHelper : IImageHelper
	{
		private readonly IWebHostEnvironment _env;
		private readonly IUnitOfWork _unitOfWork;

		public ImageHelper(IWebHostEnvironment env,  IUnitOfWork unitOfWork)
		{
			_env = env;
			_unitOfWork = unitOfWork;
		}

		public async Task<Photo> SaveImageAsync(IFormFile file, ImageType type)
		{
			if (file == null || file.Length == 0)
				throw new ArgumentException("Empty file provided");

			
			var folder = Path.Combine(_env.WebRootPath, "uploads", "images", type.ToString().ToLower());
			Directory.CreateDirectory(folder);

			
			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
			var fullPath = Path.Combine(folder, fileName);

			
			using var stream = new FileStream(fullPath, FileMode.Create);
			await file.CopyToAsync(stream);

			
			var url = $"/uploads/images/{type.ToString().ToLower()}/{fileName}";

			var image = new Photo
			{
				Url = url,
				ImageType = type,
				CreatedDate = DateTime.UtcNow
			};
		    await _unitOfWork.GetRepository<Photo>().AddAsync(image);
			await _unitOfWork.SaveAsynsc();

			return image;
		}

		public async Task DeleteImageAsync(Guid imageId)
		{
			var image = await _unitOfWork.GetRepository<Photo>().GetAsync(x=>x.Id==imageId);
			if (image == null)
				return;

			
			var relativePath = image.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
			var filePath = Path.Combine(_env.WebRootPath, relativePath);
			if (File.Exists(filePath))
				File.Delete(filePath);

			
			await _unitOfWork.GetRepository<Photo>().DeleteAsync(image);
			await _unitOfWork.SaveAsynsc();
		}
	}
}
