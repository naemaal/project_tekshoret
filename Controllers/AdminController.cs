using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoviesBooking.Models;
using MoviesBooking.DAL;
using MoviesBooking.ViewModel;
using System.IO;

namespace MoviesBooking.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult MangmentMovies()
        {
            Dal dal = new Dal();
            MovieViewModel cvm = new MovieViewModel();
            List<Movie> movies = dal.movies.ToList<Movie>();
            cvm.movie = new Movie();
            cvm.movies = movies;
            return View(cvm);
        }
        public ActionResult ManagingHalls()
        {
            return View();
        }
        public ActionResult AddHall(Hall obj)
        {
            Dal dal = new Dal();
            List<Hall> exist = (from x in dal.halls where x.hallId.Equals(obj.hallId) select x).ToList<Hall>();
            if(exist.Count != 0)
            {
                TempData["msg"] = "Hall exist !!!";
                TempData["color"] = "red";
                return View("ManagingHalls");
            }

            dal.halls.Add(obj);
            dal.SaveChanges();
            TempData["msg"] = "Hall added successfully !!!";
            TempData["color"] = "blue";
            return View("ManagingHalls");
        }
        public ActionResult UpdateHallSeats(Hall obj)
        {
            Dal dal = new Dal();
            List<Hall> exist = (from x in dal.halls where x.hallId.Equals(obj.hallId) select x).ToList<Hall>();
            if (exist.Count == 0)
            {
                TempData["msg"] = "Hall is not exist !!";
                TempData["color"] = "red";
                return View("ManagingHalls");
            }
            List<Ticket> tickets = (from x in dal.tickets from y in dal.movies 
                                    where x.movieId.Equals(y.movieId) && y.hallId.Equals(obj.hallId)
                                    select x).ToList<Ticket>();
            if (tickets.Count != 0)
            {
                TempData["msg"] = "There are already buyers in this hall !!";
                TempData["color"] = "red";
                return View("ManagingHalls");
            }
            exist[0].seatsNumber = obj.seatsNumber;
            dal.SaveChanges();
            TempData["msg"] = "Hall's seats changed successfully !!";
            TempData["color"] = "blue";
            return View("ManagingHalls");
        }
        public ActionResult RemoveMovie()
        {
            Dal dal = new Dal();
            int id;
            Int32.TryParse(Request.Form["RemoveMovie"], out id);
            List<Movie> exist = (from x in dal.movies where x.movieId.Equals(id) select x).ToList<Movie>();

            string date = exist[0].date + " " + exist[0].time;
            DateTime MDate = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm", null);

            DateTime NDate = DateTime.Now;

            var ticketsNumber = (from x in dal.tickets where x.movieId == id select x).ToList<Ticket>();

            if((NDate >=MDate) || (ticketsNumber.Count == 0))
            {

                dal.movies.Remove(exist[0]);
                dal.SaveChanges();
                return RedirectToAction("MangmentMovies", "Admin");
            }
            TempData["msg"] = "Can't delete movie , The User Already take tickets";
            TempData["color"] = "red";
            return RedirectToAction("MangmentMovies");
             
        }
        public ActionResult AddMovie()
        {
            return View(); 
        }
        public ActionResult AddNewMovie(Movie obj,Image img)
        {
            Dal dal = new Dal();
            List<Movie> exist = (from x in dal.movies 
                                 where x.time.Contains(obj.time)
                                 && x.date.Contains(obj.date) && x.hallId.Equals(obj.hallId)
                                 select x).ToList<Movie>();

            if (exist.Count != 0)
            {
                TempData["msg"] = "There is a movie running in the same time in the same hall !!";
                TempData["color"] = "red";
                return View("AddMovie");
            }
            List<Hall> halls = (from x in dal.halls
                                 where x.hallId.Equals(obj.hallId)
                                 select x).ToList<Hall>();

            if (halls.Count == 0)
            {
                TempData["msg"] = "Movie hall not exist !!";
                TempData["color"] = "red";
                return View("AddMovie");
            }


            string filename = Path.GetFileNameWithoutExtension(img.image.FileName);
            string extension = Path.GetExtension(img.image.FileName);
            filename = filename + obj.movieId + extension;
            obj.ImageUrl = "~/Images/" + filename;
            filename = Path.Combine(Server.MapPath("~/Images/"), filename);
            img.image.SaveAs(filename);
            
            dal.movies.Add(obj);
            dal.SaveChanges();
            TempData["msg"] = "Movie added successfully !!";
            TempData["color"] = "blue";
            return View("AddMovie");
        }

        public ActionResult UpdateHall()
        {
            int MovieID,HallID;
            Int32.TryParse(Request.Form["UpdateHall"], out MovieID);
            Int32.TryParse(Request.Form["NewHall"], out HallID);

            Dal dal = new Dal();
            List<Hall> halls = (from x in dal.halls
                                where x.hallId.Equals(HallID)
                                select x).ToList<Hall>();

            if (halls.Count == 0)
            {
                TempData["msg"] = "Hall not exist !!";
                TempData["color"] = "red";
                return RedirectToAction("MangmentMovies", "Admin");
            }

            List<Ticket> tickets = (from x in dal.tickets where x.movieId.Equals(MovieID) 
                                    select x).ToList<Ticket>();

            if (tickets.Count != 0)
            {
                TempData["msg"] = "There are already buyers for this movie !!"; ;
                TempData["color"] = "red";
                return RedirectToAction("MangmentMovies", "Admin");
            }

            List<Movie> movies = (from x in dal.movies
                                    where x.movieId.Equals(MovieID)
                                    select x).ToList<Movie>();

            movies[0].hallId = HallID;
            dal.SaveChanges();
            return RedirectToAction("MangmentMovies", "Admin");
        }
        public ActionResult UpdatePrice()
        {
            int MovieID;
            double price;
            Int32.TryParse(Request.Form["UpdatePrice"], out MovieID);
            double.TryParse(Request.Form["NewPrice"], out price);

            Dal dal = new Dal();
            List<Movie> movies = (from x in dal.movies
                                  where x.movieId.Equals(MovieID)
                                  select x).ToList<Movie>();

            movies[0].prePrice = movies[0].price;
            movies[0].price = price;
            dal.SaveChanges();
            return RedirectToAction("MangmentMovies", "Admin");
        }
        
    }
}