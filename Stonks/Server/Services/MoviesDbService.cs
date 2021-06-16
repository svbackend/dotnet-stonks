using System;
using Microsoft.EntityFrameworkCore;
using Stonks.Server.Data;
using Stonks.Shared.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Stonks.Server.Services
{
    public interface IMoviesDbService
    {
        Task<List<Movie>> GetMovies();
        Task<Movie> AddMovie(Movie movie);
        Task<Movie> GetMovie(int movieId);
        Task<Movie> UpdateMovie(Movie movie);
    }

    public class MoviesDbService : IMoviesDbService
    {
        private ApplicationDbContext _context;
        private IMapper _mapper;

        public MoviesDbService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Movie> AddMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            
            return movie;
        }

        public Task<Movie> GetMovie(int movieId)
        {
            return _context.Movies.SingleOrDefaultAsync(m => m.Id == movieId);
        }

        public Task<List<Movie>> GetMovies()
        {
            return _context.Movies.OrderBy(m => m.Title).ToListAsync();
        }
        
        public async Task<Movie> UpdateMovie(Movie movie)
        {
            var movieDb = await _context.Movies.SingleOrDefaultAsync(x => x.Id == movie.Id);

            if (movieDb == null)
            {
                throw new Exception();
            }
            
            _mapper.Map(movie, movieDb);
            
            _context.Entry(movieDb).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            
            return movieDb;
        }
    }
}
