using BerrasBio.Data;
using Microsoft.AspNetCore.Mvc;

namespace BerrasBio.Controllers
{
    // A controller that has the components of the whole booking process. 
    public class MovieController : Controller
    {
        private readonly DataContext _context;

        public MovieController(DataContext context)
        {
            _context = context;
        }
         
        // All movies will be listed.
        [HttpGet]
        public IActionResult MovieList()
        {
            
            var movieList = _context.Movies;

            return View(movieList);      
                
        }

        // Here the latest four movie schedules will be listed. The user can also sort by seats left or
        // starting time by clicking on the green parts of the table. The row with the most seats left will
        // be shown on top by default.
        [HttpGet]
        public IActionResult SelectTime(int id, string sorting)
        {

            var join = (from ca in _context.CinemaAuditoriums
                        join s in _context.ScheduledMovies on ca.Id equals s.AuditoriumId
                        join m in _context.Movies on s.MovieId equals m.Id

                        where m.Id == id && DateTime.Now < s.Start
                        orderby s.Start ascending

                        select new JoinModel
                        {
                            ScheduleId = s.Id,
                            Price = m.Price,
                            SeatsLeft = s.SeatsLeft,
                            Start = s.Start,
                            End = s.End,
                            Title = m.Title,
                            AuditoriumName = ca.Name


                        }).Take(4);


            ViewData["seatsLeft"] = string.IsNullOrEmpty(sorting) ? "SeatsLeft" : "";
            ViewData["start"] = string.IsNullOrEmpty(sorting) ? "Start" : "";


            switch (sorting)
            {
                case "SeatsLeft":
                    join = join.OrderBy(x => x.SeatsLeft);
                    break;
                case "Start":
                    join = join.OrderBy(x => x.Start);
                    break;

                default:
                    join = join.OrderByDescending(x => x.SeatsLeft);
                    break;
            }

            foreach (var item in join)
            {
                ViewData["title"] = item.Title;
            }

           

            return View(join);
        }
        // Here the user can choose how many seats he wants. The price will first be shown in javascript
        // but the real price will be counted in the backend when the user presses the booking button.
        [HttpGet]

        public IActionResult SelectSeat(int id)
        {

            var join = (from ca in _context.CinemaAuditoriums
                                join s in _context.ScheduledMovies on ca.Id equals s.AuditoriumId
                                join m in _context.Movies on s.MovieId equals m.Id
                               
                                
                                where s.Id == id
                                select new JoinModel
                                {
                                    Price = m.Price,
                                    SeatsLeft = s.SeatsLeft,
                                    Start = s.Start
                                    
                                    
                                });
          
            
            foreach (var item in join)
            {
                ViewData["start"] = item.Start;
                ViewData["price"] = item.Price;


                
                    if (item.SeatsLeft == 0)
                    {
                        return RedirectToAction("Error");
                    }
                
            }

            ViewData["scheduleId"] = id;


            return View(join);
        }
        // Here is what happends when the user presses the booking button.

        [HttpPost]
        public IActionResult SelectSeat()
        {

            string firstName = HttpContext.Request.Form["FirstName"];
            string lastName = HttpContext.Request.Form["LastName"];
            string mail = HttpContext.Request.Form["Mail"];
            string stringNumberOfSeats = HttpContext.Request.Form["NumberOfSeats"];
            string stringScheduleId = HttpContext.Request.Form["ScheduleId"];
            // Just a validation if the user picks ---- in the drop down on frontend. He
            // will be redirected to an error page.
            if (!stringNumberOfSeats.Any(char.IsDigit))
            {
                return RedirectToAction("Error");
            }

            int scheduleId = Convert.ToInt32(stringScheduleId);
            int numberOfSeats = Convert.ToInt32(stringNumberOfSeats);
            
            bool isTooManySeats = IsSeatNumberTooLarge(numberOfSeats, scheduleId);

            if (isTooManySeats)
            {
                return RedirectToAction("Error");
            }

            int customerId = AddCustomer(firstName, lastName, mail);
            double price = Price(scheduleId, numberOfSeats);
            int reservationId =  AddReservation(scheduleId, customerId, numberOfSeats, price);
            UpdateSeats(scheduleId, numberOfSeats);
          
            
            return RedirectToAction("Confirmation", new { id = reservationId});

        }

        // Customer will be added if he is not there already.
        public int AddCustomer(string firstName, string lastName, string mail)
        {
            var customer = _context.Customers.FirstOrDefault(m => m.Mail == mail);

            if (customer == null)
            {
                customer = new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Mail = mail
                };
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }

            return customer.Id;
        }
        // A reservation will be added.
        public int AddReservation(int scheduleId, int customerId, int numberOfSeats, double price)
        {
            var reservation = new Reservation
            {
                ScheduledMovieId = scheduleId,
                CustomerId = customerId,
                NumberOfseats = numberOfSeats,
                Price = price
                
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return reservation.Id;

        }

        
        // The number of seats left will be changed.
        public void UpdateSeats(int scheduleId, int numberOfSeats)
        {

                var seatsLeft = _context.ScheduledMovies.Find(scheduleId);

                if (seatsLeft != null)
                {
                    seatsLeft.SeatsLeft = seatsLeft.SeatsLeft - numberOfSeats;
                }
            
            _context.SaveChanges();

        }
        // The total cost will be counted here. When a movie starts between 17:30 and 23:59 a ticket
        // will be 20 kr more expensive.
        public double Price(int scheduleId, int numberOfSeats)
        {
            var join = (from m in _context.Movies
                        join s in _context.ScheduledMovies on m.Id equals s.MovieId
                 
                        where s.Id == scheduleId
                        select new 
                        {
                            Price = m.Price,
                            Start = s.Start


                        });
            double price = 0;

            foreach (var item in join)
            {
                DateTime day = item.Start;
                string stringDay = day.ToString().Substring(11, 5).Replace(':', ',');
                double time = Convert.ToDouble(stringDay);

                if (time <= 23.59 && time >= 17.3)
                {
                    price = (item.Price + 20) * numberOfSeats;

                }

                else
                {
                    price = item.Price * numberOfSeats;

                }

            }
            return price;
        }
        // Here is an action for the booking confirmation.
        public IActionResult Confirmation(int id)
        {
            var join = (from ca in _context.CinemaAuditoriums
                        join s in _context.ScheduledMovies on ca.Id equals s.AuditoriumId
                        join m in _context.Movies on s.MovieId equals m.Id
                        join r in _context.Reservations on s.Id equals r.ScheduledMovieId
                        join c in _context.Customers on r.CustomerId equals c.Id

                        where r.Id == id
                        
                        select new ConfirmationModel
                        {
                            FirstName = c.FirstName,
                            LastName = c.LastName,
                            ReservationId = r.Id,
                            Price = r.Price,
                            NumberOfSeats = r.NumberOfseats,
                            Movie = m.Title,
                            AuditoriumName = ca.Name,
                            Start = s.Start
 

                        }); 
            
            return View(join);
        }
        // Backend validation if the seat number is too large. There is also validation in the frontend
        // so the user should not even get here.
        public bool IsSeatNumberTooLarge(int numberOfSeats, int scheduleId)
        {
            var number = (from s in _context.ScheduledMovies
                          where s.Id == scheduleId

                          select new
                          {
                              SeatsLeft = s.SeatsLeft
                          });

            foreach (var item in number)
            {
                if (numberOfSeats > item.SeatsLeft || numberOfSeats > 12 || item.SeatsLeft == 0)
                {
                    return true;
                }
            }

            return false;
        }   
        // Just an action for general errors.
        public IActionResult Error()
        {
            return View();
        }

    }
}
