using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieApp.Server.Services;
using System.Threading.Tasks;
using MovieApp.Shared.DTOs;
using MovieApp.Shared.Models;
using Newtonsoft.Json.Linq;


namespace MovieApp.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly IMoviesDbService _dbService;

        public MoviesController(ILogger<MoviesController> logger, IMoviesDbService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
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
