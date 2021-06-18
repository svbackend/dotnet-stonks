using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stonks.Server.Services;
using System.Threading.Tasks;
using Stonks.Shared.DTOs;
using Stonks.Shared.Models;
using Newtonsoft.Json.Linq;


namespace Stonks.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/stocks")]
    public class StocksController : ControllerBase
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly IMoviesDbService _dbService;

        public StocksController(ILogger<MoviesController> logger, IMoviesDbService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStocks([FromQuery] string query)
        {
            // var stocks = _stocks.findByTickerOrCompany(query);
            
            /* inside:
             * 
             * var (stocks, error) = stocksDataProvider.findByTickerOrCompany(query);
             *
             * if (error != null) {
             *     stocks = stocksRepository.findByTickerOrCompany(query);
             * }
             *
             * return stocks;
             */
            
            /*
             * <IEnumerable<StockDto>, IError> findByTickerOrCompany(string query) {
             *    IEnumerable<StockDto> stocks;
             *    try {
             *      stocks = apiClient.search(query)
             *    } catch (ApiLimitExceededException e) {
             *       return (null, new Error(...));
             *    }
             *
             *    todo cache results
             * 
             *    return stocks;
             * }
             */
            
            
            return Ok(await _dbService.GetMovies());
        }

        [HttpGet("{idMovie}")]
        public async Task<IActionResult> GetMovie(int idMovie)
        {
            return Ok(await _dbService.GetMovie(idMovie));
        }

        [HttpPut("{idMovie}")]
        public async Task<IActionResult> UpdateMovie(int idMovie, Movie movie)
        {
            await _dbService.UpdateMovie(movie);
            
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie(Movie movie)
        {
            await _dbService.AddMovie(movie);
            return Ok(await _dbService.GetMovies());
        }
    }
}
