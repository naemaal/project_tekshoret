using MoviesBooking.DAL;
using MoviesBooking.Models;
using MoviesBooking.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MoviesBooking.Controllers
{
    public class UserController : Controller
    {
        public ActionResult index()
        {
            return RedirectToAction("ShowHowPage");
        }
        // GET: User
        public ActionResult ShowHowPage()
        {

            Dal dal = new Dal();
            MovieViewModel cvm = new MovieViewModel();
            List<Movie> movies = dal.movies.ToList<Movie>();
            cvm.movie = new Movie();
            cvm.movies = movies;
            if (Session["UserName"] == null)
            {
                Guest g = new Guest() { };
                dal.guests.Add(g);
                dal.SaveChanges();
                List<Guest> guests = dal.guests.ToList<Guest>();
                Session["UserName"] = "??"+guests[guests.Count - 1].Id.ToString();
            }
            return View(cvm);
        }
        public ActionResult LoginUsers()
        {
            return View();
        }
        public ActionResult RegisterUsers()
        {
            return View();
        }
        public ActionResult AddNewUser(User obj)
        {
            Dal dal = new Dal();
            List<User> exist = (from x in dal.users where x.userName.Contains(obj.userName) select x).ToList<User>();
            if (exist.Count != 0)
            {
                TempData["msg"] = "User already exist !!";
                TempData["color"] = "red";
                return View("RegisterUsers");
            }

            TempData["msg"] = "User created successfully !!!";
            TempData["color"] = "blue";
            dal.users.Add(obj);
            dal.SaveChanges();
            return View("RegisterUsers");
        }
        public ActionResult SingIn(User obj)
        {
            Dal dal = new Dal();
            List<User> exist = (from x in dal.users where x.userName.Equals(obj.userName)
                                && x.password.Equals(obj.password)
                                select x).ToList<User>();
            if (exist.Count == 0)
            {
                TempData["msg"] = "Wrong information !!";
                TempData["color"] = "red";
                return View("LoginUsers");
            }

            if (exist[0].userType == "A")
                return RedirectToAction("MangmentMovies", "Admin");
            else
            {
                Session["UserName"] = obj.userName;
                return RedirectToAction("ShowHowPage", "User");
            }
        }
        public ActionResult ChangeSeat()
        {
            var dal = new Dal();

            var TicketId=int.Parse(Request.Form["ticketId2"]);

            Session["TicketID"] = TicketId;

            var movieId = (from x in dal.tickets
                           where x.ticketId == TicketId
                           select x.movieId).ToList<int>()[0];

            Movie movie = (from x in dal.movies
                           where x.movieId == movieId
                           select x).ToList<Movie>()[0];

            Session["TicketMovieID"] = movieId;
            var hall = (from x in dal.movies
                        from y in dal.halls
                        where x.movieId == movie.movieId
                        && y.hallId == x.hallId
                        select y).ToList<Hall>()[0];

            // get all not avalibale tickets
            var notAvalibaleTickets = (from x in dal.tickets
                                       where x.movieId == movie.movieId
                                       select x).ToList<Ticket>();

            notAvalibaleTickets = (from x in notAvalibaleTickets
                                   orderby x.seatNumber
                                   select x).ToList<Ticket>();

            var list1 = Enumerable.Range(1, hall.seatsNumber).ToList();
            var list2 = (from x in notAvalibaleTickets
                         select x.seatNumber).ToList<int>();
            var list3 = list1.Except(list2).ToList();
            if (list3.Count != 0)
            {
                SeatsViewModel seatView = new SeatsViewModel()
                {
                    movie = movie,
                    tickets = notAvalibaleTickets,
                    hall = hall
                };
                return View(seatView);
            }
         
            TempData["msg"] = "Can't Change the seat , You Take the last seat.!!";
            TempData["color"] = "red";
            return RedirectToAction("Cart");
        }
        public ActionResult HallSeats()
        {

            var dal = new Dal();

            var movieId = int.Parse(Request.Form["movieId"]);
            Movie movie = (from x in dal.movies
                         where x.movieId == movieId
                         select x).ToList<Movie>()[0];

            Session["TicketMovieID"] = movieId;
            var hall = (from x in dal.movies
                        from y in dal.halls
                        where x.movieId == movie.movieId
                        && y.hallId == x.hallId
                        select y).ToList<Hall>()[0];

            // get all not avalibale tickets
            var notAvalibaleTickets = (from x in dal.tickets
                                       where x.movieId == movie.movieId
                                       select x).ToList<Ticket>();

            notAvalibaleTickets = (from x in notAvalibaleTickets
                                   orderby x.seatNumber
                                   select x).ToList<Ticket>();
            SeatsViewModel seatView = new SeatsViewModel()
            {
                movie = movie,
                tickets = notAvalibaleTickets,
                hall = hall
            };
            string date = movie.date + " " + movie.time;
            DateTime MDate = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm", null);
            DateTime NDate = DateTime.Now;
            if (NDate >= MDate)

            {
                TempData["msg"] = "You cant buy a ticket for movie that ended!!";
                TempData["color"] = "red";
                return RedirectToAction("ShowHowPage", "User"); ;
            }
            if (Session["age"]!= null && movie.age >(int)Session["age"])

            {
                TempData["msg"] = "You are under the requested age !!";
                TempData["color"] = "red";

                return RedirectToAction("ShowHowPage", "User"); ;
            }

            var list1 = Enumerable.Range(1, hall.seatsNumber).ToList();
            var list2 = (from x in notAvalibaleTickets
                         select x.seatNumber).ToList<int>();
            var list3 = list1.Except(list2).ToList();
            if (list3.Count != 0)
            {
                return View(seatView);
            }

            TempData["msg"] = "Can't Buy a ticket , All tickets are bougth.!!";
            TempData["color"] = "red";
            return RedirectToAction("ShowHowPage");
        }
        public ActionResult OrderMovies()
        {
            Dal dal = new Dal();
            MovieViewModel cvm = new MovieViewModel();
            string choice = (string)Request.Form["OrderBy"];
            List<Movie> movies;
            switch (choice)
            {
                case "Price increase":
                    movies = dal.movies.OrderBy(c => c.price).ToList<Movie>();
                    break;
                case "Price decrease":
                    movies = dal.movies.OrderByDescending(x => x.price).ToList<Movie>();
                    break;
                case "Most popular":
                    int i, j, min;
                    Movie temp;
                    List<Movie> AllMovies= dal.movies.ToList<Movie>();
                    for (i = 0; i < AllMovies.Count-1; i++)
                    {
                        min = i;
                        for (j = i + 1; j < AllMovies.Count; j++)
                            if (GetPopular(AllMovies[j].movieId) >= GetPopular(AllMovies[min].movieId))
                                min = j;
                        temp = AllMovies[i];
                        AllMovies[i] = AllMovies[min];
                        AllMovies[min] = temp;
                    }
                    movies = AllMovies;
                    break;
                default:
                    movies = dal.movies.OrderBy(x => x.category).ToList<Movie>();
                    break;
            }
            cvm.movie = new Movie();
            cvm.movies = movies;
            return View("ShowHowPage", cvm);
        }
        public int GetPopular(int id)
        {
            Dal dal = new Dal();
            List<Ticket> tickets = (from x in dal.tickets where x.movieId.Equals(id) select x).ToList<Ticket>();
            return tickets.Count;
        }
        public ActionResult BuyBefore()
        {
            int id, seatNum;
            id = (int)Session["TicketMovieID"];
            Int32.TryParse(Request.Form["SeatNumber"], out seatNum);
            Dal dal = new Dal();
            Ticket ticket = new Ticket
            { isPayed = false, seatNumber = seatNum, movieId = id, userName = (string)Session["UserName"] };

            dal.tickets.Add(ticket);
            dal.SaveChanges();
            List<Ticket> tickets = dal.tickets.ToList<Ticket>();
            Session["BuyTicket"] = tickets[tickets.Count-1].ticketId.ToString();
            return View("Payment");
        }
        public ActionResult SearchMovie()
        {
            return View();
        }
        
        public ActionResult SearchByCategory()
        {

            Dal dal = new Dal();
            string category=Request.Form["MovieCategory"];
            MovieViewModel cvm = new MovieViewModel();
            List<Movie> movies = (from x in dal.movies where x.category.Equals(category)
                                  select x).ToList<Movie>();
            cvm.movie = new Movie();
            cvm.movies = movies;
            return View("ShowHowPage", cvm);
        }
        public ActionResult SearchByTime()
        {

            Dal dal = new Dal();
            string time = Request.Form["MovieTime"];
            MovieViewModel cvm = new MovieViewModel();
            List<Movie> movies = (from x in dal.movies
                                  where x.time.Equals(time)
                                  select x).ToList<Movie>();
            cvm.movie = new Movie();
            cvm.movies = movies;
            return View("ShowHowPage", cvm);
        }
        public ActionResult SearchByDate()
        {

            Dal dal = new Dal();
            string date = Request.Form["MovieDate"];
            MovieViewModel cvm = new MovieViewModel();
            List<Movie> movies = (from x in dal.movies
                                  where x.time.Equals(date)
                                  select x).ToList<Movie>();
            cvm.movie = new Movie();
            cvm.movies = movies;
            return View("ShowHowPage",cvm);
        }
        public ActionResult SearchByPrice()
        {

            Dal dal = new Dal();
            double price;
            double.TryParse(Request.Form["rangeInput"], out price);
            MovieViewModel cvm = new MovieViewModel();
            List<Movie> movies = (from x in dal.movies
                                  where x.price <= price 
                                  select x).ToList<Movie>();
            cvm.movie = new Movie();
            cvm.movies = movies;
            return View("ShowHowPage", cvm);
        }
        public ActionResult FilterMovie()
        {

            Dal dal = new Dal();
            MovieViewModel cvm = new MovieViewModel();
            List<Movie> movies = (from x in dal.movies
                                  where x.prePrice !=0
                                  select x).ToList<Movie>();
            cvm.movie = new Movie();
            cvm.movies = movies;
            return View("ShowHowPage", cvm);
        }
        public ActionResult Cart()
        {
            Dal dal = new Dal();
            string UserName = (string)Session["UserName"];
            List<Ticket> tickets = (from x in dal.tickets where x.userName.Equals(UserName)
                                    select x).ToList<Ticket>();
            TicketViewModel cvm = new TicketViewModel();
            cvm.tickets = tickets;
            return View(cvm);
        }

        public ActionResult SignOut()
        {
            Session["UserName"] = null;
            return RedirectToAction("ShowHowPage","User");
        }
        public ActionResult Payment()
        {
            Session["BuyTicket"] = Request.Form["ticketId1"];
            return View();
        }
        public ActionResult PaymentAll()
        {
            return View();
        }
        public ActionResult BuyTicket()
        {
            int id;
            Int32.TryParse((string)Session["BuyTicket"], out id);
            Dal dal = new Dal();
            List<Ticket> tickets = (from x in dal.tickets
                                    where x.ticketId.Equals(id)
                                    select x).ToList<Ticket>();
            tickets[0].isPayed = true;
            dal.SaveChanges();
            TempData["msg"] = "Payment done successfully !!";
            TempData["color"] = "blue";
            return RedirectToAction("Cart", "User");
        }
        public ActionResult BuyAllTickets()
        {

            string id = (string)Session["UserName"];
            Dal dal = new Dal();
            List<Ticket> tickets = (from x in dal.tickets
                                    where x.userName.Equals(id)
                                    select x).ToList<Ticket>();

            foreach (Ticket ticket in tickets)
                  ticket.isPayed = true;

            dal.SaveChanges();
            TempData["msg"] = "Payment done successfully !!";
            TempData["color"] = "blue";
            return RedirectToAction("Cart", "User");
        }

        public ActionResult CreatTicket()
        {
            int id, seatNum;
            id =(int) Session["TicketMovieID"];
            Int32.TryParse(Request.Form["SeatNumber"], out seatNum);
            Dal dal = new Dal();
            Ticket ticket = new Ticket
            {isPayed = false, seatNumber = seatNum, movieId = id, userName = (string)Session["UserName"] };

            dal.tickets.Add(ticket);
            dal.SaveChanges();
            TempData["msg"] = "Item added to Cart Susseccfuly!!";
            TempData["color"] = "blue";
            return RedirectToAction("ShowHowPage", "User");
        }
        public ActionResult UpdateTicket()
        {
            int id, seatNum;
            id = (int)Session["TicketID"];

            Int32.TryParse(Request.Form["SeatNumber"], out seatNum);
            Dal dal = new Dal();
            Ticket ticket = (from x in dal.tickets
                                    where x.ticketId.Equals(id)
                                    select x).ToList<Ticket>()[0];
            ticket.seatNumber = seatNum;
            dal.SaveChanges();
            return RedirectToAction("Cart", "User");
        }

    }
}