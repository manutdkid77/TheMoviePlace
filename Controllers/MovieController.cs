using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheMoviePlace.Entities;
using TheMoviePlace.Models;
using System.Collections.Generic;

namespace TheMoviePlace.Controllers
{
    public class MovieController : Controller
    {
        TheMoviePlaceDBContext _TheMoviePlaceDBContext;
        public MovieController(TheMoviePlaceDBContext TheMoviePlaceDBContext)
        {
            _TheMoviePlaceDBContext = TheMoviePlaceDBContext;
        }
        public async Task<IActionResult> Index(){

            var lstMovies =   await _TheMoviePlaceDBContext.Movies.Include(movie=>movie.Roles).ThenInclude(role=>role.Person).ToListAsync();

            if(lstMovies.Count() == 0)
                return NotFound();

            return View(lstMovies);
        }
    }
}