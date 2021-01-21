using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MoviesBooking.Models
{
    public class Ticket
    {
        [Key]
        public int ticketId { get; set; }
        public int movieId { get; set; }
        public int seatNumber { get; set; }
        public string userName { get; set; }
        public bool isPayed { get; set; }
    }
}