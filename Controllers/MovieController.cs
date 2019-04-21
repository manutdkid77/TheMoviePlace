using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheMoviePlace.Entities;
using TheMoviePlace.Helpers;
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
                var lstMovies = await _TheMoviePlaceDBContext.Movies.Include (movie => movie.Roles).ThenInclude (role => role.Person).Select (m => new Movie () {
                    Name = m.Name,
                        MovieID = m.MovieID,
                        YearOfRelease = m.YearOfRelease,
                        Roles = m.Roles,
                        Poster = string.IsNullOrEmpty (m.Poster) ? Path.Combine (StringConstants.DefaultImagesFolder, StringConstants.MovieNoImageName) : Path.Combine (StringConstants.FileUploadFolder, m.Poster)
                }).AsNoTracking ().ToListAsync ();

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
        [RequestSizeLimit (5000000)]
        public async Task<IActionResult> AddMovie ([FromForm] AddMovieViewModel oAddMovieModel) {
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

                    strFileName = Guid.NewGuid () + _fileProcessService.GetFileExtension (oAddMovieModel.Poster.FileName);

                    await _fileProcessService.UploadFile (oAddMovieModel.Poster, strFileName, ModelState);

                    if (!ModelState.IsValid)
                        return BadRequest (ModelState);
                }

                var oMovie = new Movie () {
                    Name = oAddMovieModel.Name,
                    Plot = oAddMovieModel.Plot,
                    Poster = strFileName,
                    YearOfRelease = oAddMovieModel.YearOfRelease
                };

                await _TheMoviePlaceDBContext.Movies.AddAsync (oMovie);
                await _TheMoviePlaceDBContext.SaveChangesAsync ();

                if (oAddMovieModel.Actors.Count > 0)
                    await InsertRolesForMovie (oAddMovieModel.Actors, oMovie.MovieID, 1);

                if (oAddMovieModel.Producers.Count > 0)
                    await InsertRolesForMovie (oAddMovieModel.Producers, oMovie.MovieID, 2);

                return RedirectToAction (nameof (Index));
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

        private async Task InsertRolesForMovie (List<int> lstPersons, int iMovieID, int iRoleReferenceID) {
            var lstRoles = new List<Role> ();

            foreach (var iPersonID in lstPersons) {
                lstRoles.Add (new Role () { MovieID = iMovieID, PersonID = iPersonID, RoleReferenceID = iRoleReferenceID });
            }

            await _TheMoviePlaceDBContext.AddRangeAsync (lstRoles);
            await _TheMoviePlaceDBContext.SaveChangesAsync ();
        }

        public async Task<IActionResult> EditMovie(int? MovieID){
            try{
                if(MovieID is null)
                    return NotFound();
                
                var oMovie = await _TheMoviePlaceDBContext.Movies.Include(r => r.Roles).ThenInclude(p => p.Person).AsNoTracking().FirstOrDefaultAsync(m => m.MovieID == MovieID);

                if(oMovie is null)
                    return NotFound();
                
                var oEditViewModel = new EditMovieViewModel()
                {
                    MovieID = oMovie.MovieID,
                    Name = oMovie.Name,
                    PosterUrl = oMovie.Poster,
                    Plot = oMovie.Plot,
                    YearOfRelease = oMovie.YearOfRelease,
                    Roles = oMovie.Roles
                };

                return View(oEditViewModel);
            }
            catch(Exception ex){
                _loggerService.LogError(ex,ex.Message);
                return View();
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [RequestSizeLimit (5000000)]
        public async Task<IActionResult> EditMovie([FromForm] EditMovieViewModel oEditMovieModel){
            try{
                var strFileName = string.Empty;

                if (oEditMovieModel is null || oEditMovieModel.MovieID <=0)
                    return BadRequest ();

                if (!ModelState.IsValid)
                    return BadRequest (ModelState);

                var oMovie = await _TheMoviePlaceDBContext.Movies.FirstOrDefaultAsync(m=>m.MovieID ==oEditMovieModel.MovieID);

                oMovie.Name = oEditMovieModel.Name;
                oMovie.Plot = oEditMovieModel.Plot;
                oMovie.YearOfRelease = oEditMovieModel.YearOfRelease;

                if (oEditMovieModel.Poster != null) {

                    _fileProcessService.ProcessFormFile (oEditMovieModel.Poster, ModelState);

                    if (!ModelState.IsValid)
                        return BadRequest (ModelState);

                    strFileName = string.IsNullOrEmpty(oMovie.Poster) ? Guid.NewGuid().ToString() + _fileProcessService.GetFileExtension (oEditMovieModel.Poster.FileName) : oMovie.Poster;

                    await _fileProcessService.UploadFile (oEditMovieModel.Poster, strFileName, ModelState);

                    if (!ModelState.IsValid)
                        return BadRequest (ModelState);
                    
                    oMovie.Poster = strFileName;
                }

                await _TheMoviePlaceDBContext.SaveChangesAsync ();
                
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex){
                _loggerService.LogError(ex,ex.Message);
                return View();
            }
        }
    }
}