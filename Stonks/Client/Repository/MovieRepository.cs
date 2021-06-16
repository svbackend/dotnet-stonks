using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stonks.Shared.Models;
using Stonks.Client.Helpers;

namespace Stonks.Client.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IHttpService _httpService;
        private const string Url = "/api/movies";

        public MovieRepository(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<List<Movie>> GetMovies()
        {
            var response = await _httpService.Get<List<Movie>>(Url);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task<Movie> GetMovie(int id)
        {
            var response = await _httpService.Get<Movie>($"{Url}/{id}");

            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task CreateMovie(Movie movie)
        {
            var response = await _httpService.Post(Url, movie);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task UpdateMovie(Movie movie)
        {
            var response = await _httpService.Put($"{Url}/{movie.Id}", movie);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task DeleteMovie(int id)
        {
            var response = await _httpService.Delete($"{Url}/{id}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }
    }
}
