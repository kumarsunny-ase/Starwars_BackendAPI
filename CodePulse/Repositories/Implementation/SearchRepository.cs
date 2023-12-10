using System;
using CodePulse.Data;
using CodePulse.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.Repositories.Implementation
{
	public class SearchRepository
	{
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _applicationDbContext;

        public SearchRepository(HttpClient httpClient, ApplicationDbContext applicationDbContext)
        {
            _httpClient = httpClient;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<string> GetPeopleData()
        {
            // SWAPI endpoint for people
            var apiUrl = "https://swapi.dev/api/films/{query}";

            // Make a GET request to the SWAPI endpoint
            var response = await _httpClient.GetStringAsync(apiUrl);

            return response;
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

