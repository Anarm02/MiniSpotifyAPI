using EntityLayer.Entities;
using EntityLayer.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Helpers.ImageHelpers
{
	public interface IImageHelper
	{
		Task<Photo> SaveImageAsync(IFormFile file, ImageType type);
		Task DeleteImageAsync(Guid imageId);
	}
}
