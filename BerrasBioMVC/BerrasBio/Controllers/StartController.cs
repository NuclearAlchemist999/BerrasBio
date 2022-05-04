using BerrasBio.Data;
using BerrasBio.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BerrasBio.Controllers
{
    // A controller that has the components of the start page and info page.
    public class StartController : Controller
    {
      
        private readonly DataContext _context;

        public StartController(DataContext context)
        {
            _context = context;
        }
        // Here the latest six movie schedules will be listed. If I only wanted to show the list of the movies
        // scheduled today I could just use where DateTime.Today == s.Start.Date in the linq query but since
        // that would require a lot of test data for movies everyday, the latest schedules will be shown instead.
        // There is also a sorting list where the latest starting time will be shown on top by default
        // and the user can also sort by seats left descending or the reverse starting time by clicking on the
        // green or red text.
        public IActionResult MovieSchedule(string sorting)
        {
            var join = (from ca in _context.CinemaAuditoriums
                        join s in _context.ScheduledMovies on ca.Id equals s.AuditoriumId
                        join m in _context.Movies on s.MovieId equals m.Id

                        where DateTime.Now < s.Start || DateTime.Now >= s.Start && DateTime.Now <= s.End
                     //   where DateTime.Today == s.Start.Date
                        orderby s.Start ascending

                        select new JoinModel
                        {
                            Price = m.Price,
                            SeatsLeft = s.SeatsLeft,
                            Start = s.Start,
                            End = s.End,
                            Title = m.Title,
                            AuditoriumName = ca.Name


                        }).Take(6);


            ViewData["seatsLeft"] = string.IsNullOrEmpty(sorting) ? "SeatsLeft" : "";
            ViewData["start"] = string.IsNullOrEmpty(sorting) ? "Start" : "";


            switch (sorting)
            {
                case "SeatsLeft":
                    join = join.OrderByDescending(x => x.SeatsLeft);
                    break;
                case "Start":
                    join = join.OrderByDescending(x => x.Start);
                    break;

                default:
                    join = join.OrderBy(x => x.Start);
                    break;
            }



            return View(join);

        }
        // Just some information about the page.
        public IActionResult About()
        {
            return View();
        }

        
    }
}