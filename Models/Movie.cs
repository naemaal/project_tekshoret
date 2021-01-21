using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MoviesBooking.Models
{
    public class Movie
    {
        [Key]
        public int movieId { get; set; }
        public string title { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public double price { get; set; }
        public double prePrice { get; set; }
        public string category { get; set; }
        public int hallId { get; set; }
        public int age { get; set; }
        public string ImageUrl { get; set; }
    }
}