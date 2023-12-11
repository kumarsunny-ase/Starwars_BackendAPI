using CodePulse.Models.Domain;
using CodePulse.Models.DTO;
using CodePulse.Repositories.Implementation;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]

public class FilmsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SearchRepository _searchRepository;

    public FilmsController(IHttpClientFactory httpClientFactory, SearchRepository searchRepository)
    {
        _httpClientFactory = httpClientFactory;
        _searchRepository = searchRepository;
    }

    [Route("api/films")]
    [HttpPost]
    public async Task<ActionResult<(string StarshipName, string StarshipLength, string PersonName, string PersonHeight)>> GetExpensiveStarship([FromBody] Url url)
    {
        using (var client = _httpClientFactory.CreateClient())
        {
            // Make a GET request to retrieve film data based on the provided url
            HttpResponseMessage response = await client.GetAsync(url.url);

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the JSON response to a Film object
                Film film = await response.Content.ReadFromJsonAsync<Film>();

                // Get and return the starship with the biggest length
                var biggestData = await GetBiggestData(film.Starships, film.Characters);
                return Ok(new { starshipName = biggestData.StarshipName , personName = biggestData.PersonName});
            }
            else
            {
                return NotFound();
            }
        }
    }

    
private async Task<(string StarshipName, string StarshipLength, string PersonName, string PersonHeight)> GetBiggestData(List<string> starshipUrls, List<string> characters)
    {
        using (var client = _httpClientFactory.CreateClient())
        {
            // Make parallel requests to retrieve starship data
            var starshiptasks = starshipUrls.Select(async url =>
            {
                var starship = await client.GetFromJsonAsync<Starship>(url);
                return (starship?.Name, starship?.Length);

            });

            var persontasks = characters.Select(async url =>
            {
                var person = await client.GetFromJsonAsync<Person>(url);
                return (person?.Name, person?.Height);

            });

            var starshipsData = await Task.WhenAll(starshiptasks);
            var personData = await Task.WhenAll(persontasks);

            // Filter out null values and find the starship with the maximum length
            var maxStarship = starshipsData
                .Where(data => data.Length != null)
                .OrderByDescending(data =>
                {
                    return double.TryParse(data.Length, out var parsedLength) ? parsedLength : 0;
                })
                .FirstOrDefault();

            var maxHeight = personData
                .Where(data => data.Height != null)
                .OrderByDescending(data =>
                {
                    return double.TryParse(data.Height, out var parsedHeight) ? parsedHeight: 0;
                })
                .FirstOrDefault();

            return (maxStarship.Name, maxStarship.Length, maxHeight.Name, maxHeight.Height);
        }
    }

    [Route("search/history")]
    [HttpPost]
    public async Task<ActionResult> AddSearchHistory(Search search)
    {
        var request = new Search
        {
            keyword = search.keyword,
            result = search.result,
            type = search.type
        };

        await _searchRepository.CreateAsync(request);

        var responseList = new List<SearchDto>
        {
            new SearchDto
            {
                keyword = search.keyword,
                result = search.result,
                type = search.type
            }
        };

        return Ok(responseList);

    }

    [Route("search/history")]
    [HttpGet]
    public async Task<IActionResult> GetLastHistory( string resourceType)
    {
        var histories = await _searchRepository.GetAllAsync();
        var selectedHistory = histories.OrderByDescending(h => h.Id);

        var lastFiveHistories = selectedHistory.Where( a => a.type == resourceType).Take(5);

        var response = new List<SearchDto>();
        foreach(var history in lastFiveHistories)
        {
            response.Add(new SearchDto
            {
                keyword = history.keyword,
                result = history.result,
                type = history.type
            });
        };

        return Ok(new { history = response });

    }


}