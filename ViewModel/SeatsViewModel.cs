using MoviesBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoviesBooking.ViewModel
{
    public class SeatsViewModel
    {
        public Movie movie { get; set; }

        public List<Ticket> tickets { get; set; }

        public Hall hall { get; set; }
        public int row()
        {
            return hall.seatsNumber / 4;
        }
        public int column()
        {
            return hall.seatsNumber - row();
        }
    }
}