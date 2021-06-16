using Stonks.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Repository
{
    public interface IMoviesRepository
    {
        Task<int> CreateMovie(Movie movie);
        Task DeleteMovie(int Id);
        Task<Movie> GetDetailsMovieDTO(int id);
        Task<List<Movie>> GetMovies();
        Task UpdateMovie(Movie movie);
    }
}
