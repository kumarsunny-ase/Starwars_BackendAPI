using System;
using CodePulse.Data;
using CodePulse.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.Repositories.Implementation
{
	public class SearchRepository
	{
        private readonly ApplicationDbContext _applicationDbContext;

        public SearchRepository( ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Search> CreateAsync(Search search)
        {
            await _applicationDbContext.searchs.AddAsync(search);
            await _applicationDbContext.SaveChangesAsync();

            return search;
        }

        public async Task<IEnumerable<Search>> GetAllAsync()
        {
            return await _applicationDbContext.searchs.ToListAsync();
        }
    }
}

