using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheMoviePlace.Entities;
using TheMoviePlace.Models;

namespace TheMoviePlace.Controllers {
    public class MovieController : Controller {
        TheMoviePlaceDBContext _TheMoviePlaceDBContext;
        ILogger<MovieController> _loggerService;
        public MovieController (TheMoviePlaceDBContext TheMoviePlaceDBContext, ILogger<MovieController> loggerService) {
            _TheMoviePlaceDBContext = TheMoviePlaceDBContext;
            _loggerService = loggerService;
        }
        public async Task<IActionResult> Index () {

            try {
                var lstMovies = await _TheMoviePlaceDBContext.Movies.Include (movie => movie.Roles).ThenInclude (role => role.Person).ToListAsync ();

                if (lstMovies.Count () == 0)
                    return NotFound ();

                return View (lstMovies);
            } catch (Exception ex) {
                _loggerService.LogError (ex, ex.Message);
                return View ();
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult AddMovie (AddMovieViewModel oAddMovieModel) {
            try {
                return View ();
            } catch (Exception ex) {
                _loggerService.LogError (ex, ex.Message);
                return View ();
            }
        }

        public IActionResult AddMovie () {
            try {
                return View ();
            } catch (Exception ex) {
                _loggerService.LogError (ex, ex.Message);
                return View ();
            }
        }
    }
}