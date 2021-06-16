using System.Collections.Generic;
using System.Threading.Tasks;
using MovieApp.Shared.Models;

namespace MovieApp.Client.Repository
{
    public interface IMovieRepository
    {
        Task CreateMovie(Movie movie);
        Task<Movie> GetMovie(int id);
        Task<List<Movie>> GetMovies();
        Task UpdateMovie(Movie genre);
        Task DeleteMovie(int id);
    }
}
