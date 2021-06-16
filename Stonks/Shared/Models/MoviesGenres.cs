using System.ComponentModel.DataAnnotations.Schema;

namespace Stonks.Shared.Models
{
    public class MoviesGenres
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }
        public Movie Movie { get; set; }
        public Genre Genre { get; set; }
    }
}
