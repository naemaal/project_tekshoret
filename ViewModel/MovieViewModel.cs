using System.Collections.Generic;
using MoviesBooking.Models;

namespace MoviesBooking.ViewModel
{
    public class MovieViewModel
    {
        public Movie movie { get; set; }
        public List<Movie> movies { get; set; }
    }
}