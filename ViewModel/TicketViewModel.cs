using System.Collections.Generic;
using MoviesBooking.Models;
using MoviesBooking.DAL;
using System.Linq;


namespace MoviesBooking.ViewModel
{
    public class TicketViewModel
    {
        public Ticket ticket { get; set; }
        public List<Ticket> tickets { get; set; }

        public string GetName(int id)
        {
            Dal dal = new Dal();
            List<Movie> movies = (from x in dal.movies where x.movieId.Equals(id)
                                  select x).ToList<Movie>();
            return movies[0].title;
        }
        public string GetTime(int id)
        {
            Dal dal = new Dal();
            List<Movie> movies = (from x in dal.movies
                                  where x.movieId.Equals(id)
                                  select x).ToList<Movie>();
            return movies[0].time;
        }
        public string GetDate(int id)
        {
            Dal dal = new Dal();
            List<Movie> movies = (from x in dal.movies
                                  where x.movieId.Equals(id)
                                  select x).ToList<Movie>();
            return movies[0].date;
        }
        public double GetPrice(int id)
        {
            Dal dal = new Dal();
            List<Movie> movies = (from x in dal.movies
                                  where x.movieId.Equals(id)
                                  select x).ToList<Movie>();
            return movies[0].price;
        }
        public bool isAllPayed()
        {
            var rsult = (from x in tickets where x.isPayed == false select x).ToList<Ticket>();
            return rsult.Count == 0;
        }
    }

}

