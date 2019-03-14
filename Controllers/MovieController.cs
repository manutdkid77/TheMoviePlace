using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheMoviePlace.Entities;
using TheMoviePlace.Models;
using TheMoviePlace.Services;

namespace TheMoviePlace.Controllers {
    public class MovieController : Controller {
        TheMoviePlaceDBContext _TheMoviePlaceDBContext;
        ILogger<MovieController> _loggerService;
        IFileProcessService _fileProcessService;
        public MovieController (TheMoviePlaceDBContext TheMoviePlaceDBContext, ILogger<MovieController> loggerService, IFileProcessService fileProcessService) {
            _TheMoviePlaceDBContext = TheMoviePlaceDBContext;
            _loggerService = loggerService;
            _fileProcessService = fileProcessService;
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
        [RequestSizeLimit(5000000)]
        public async Task<IActionResult> AddMovie ([FromForm]AddMovieViewModel oAddMovieModel) {
            try {
                var strFileName = string.Empty;

                if (oAddMovieModel is null)
                    return BadRequest ();

                if (!ModelState.IsValid)
                    return BadRequest (ModelState);

                if (oAddMovieModel.Poster != null) {

                    _fileProcessService.ProcessFormFile (oAddMovieModel.Poster, ModelState);

                    if (!ModelState.IsValid)
                        return BadRequest (ModelState);
                    
                    strFileName = Guid.NewGuid() + _fileProcessService.GetFileExtension (oAddMovieModel.Poster.FileName);

                    await _fileProcessService.UploadFile(oAddMovieModel.Poster,strFileName,ModelState);

                    if(!ModelState.IsValid)
                        return BadRequest(ModelState);
                }
                

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