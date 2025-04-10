using DataAccessLayer.UnitOfWorks;
using EntityLayer.DTOs.SearchDtos;
using EntityLayer.DTOs.SongDtos;
using EntityLayer.DTOs.UserDtos;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Concrete
{
	public class SearchService : ISearchService
	{
		private readonly IUnitOfWork _unitOfWork;

		public SearchService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<SearchResultDto> SearchAsync(string query)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return new SearchResultDto();
			}

			query = query.ToLower().Trim();

			var songRepository = _unitOfWork.GetRepository<Song>();
			var songs = await songRepository.GetAllAsync(
				predicate: s => s.Title.ToLower().Contains(query) && !s.IsDeleted,
				include: queryable => queryable.Include(s => s.Artists)
			);

			var songDtos = songs.Select(s => new SongDto
			{
				Id = s.Id,
				Title = s.Title,
				FilePath = s.FilePath,
				Duration = s.Duration,
				ArtistNames = s.Artists.Select(a => a.FullName).ToList()
			}).ToList();

			var userRepository = _unitOfWork.GetRepository<AppUser>();
			var artists = await userRepository.GetAllAsync(
				predicate: a => a.FullName.ToLower().Contains(query) && a.Songs.Any() ,
				include: queryable => queryable.Include(a => a.Songs).ThenInclude(s => s.Artists)
			);

			var artistDtos = artists.Select(a => new ArtistDto
			{
				Id = a.Id,
				FullName = a.FullName,
				Songs = a.Songs.Select(s => new SongDto
				{
					Id = s.Id,
					Title = s.Title,
					FilePath = s.FilePath,
					Duration = s.Duration,
					ArtistNames = s.Artists.Select(art => art.FullName).ToList()
				}).ToList()
			}).ToList();

			return new SearchResultDto
			{
				Songs = songDtos,
				Users = artistDtos
			};
		}
	}
}
