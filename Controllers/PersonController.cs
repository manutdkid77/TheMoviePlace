using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheMoviePlace.Entities;
using TheMoviePlace.Helpers;

namespace TheMoviePlace.Controllers {
    public class PersonController : Controller {
        TheMoviePlaceDBContext _TheMoviePlaceDBContext;
        ILogger<PersonController> _loggerService;
        public PersonController (TheMoviePlaceDBContext TheMoviePlaceDBContext, ILogger<PersonController> loggerService) {
            _TheMoviePlaceDBContext = TheMoviePlaceDBContext;
            _loggerService = loggerService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPerson ([FromBody] Person oPerson) {
            try {
                if (oPerson is null)
                    return BadRequest ();

                if (!ModelState.IsValid)
                    return BadRequest (ModelState);

                _TheMoviePlaceDBContext.Persons.Add (oPerson);

                await _TheMoviePlaceDBContext.SaveChangesAsync ();

                return Ok (oPerson);
            } catch (Exception ex) {
                _loggerService.LogError (ex, ex.Message);
                return StatusCode (500, StringConstants.ProcessingError);
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchPerson (string Name) {
            try {

                if (string.IsNullOrEmpty (Name) || string.IsNullOrWhiteSpace (Name))
                    return BadRequest ("Name is required");

                var lstPersons = await _TheMoviePlaceDBContext.Persons.Where (p => p.Name.Contains (Name)).OrderBy (p => p.Name).ToListAsync ();

                if (lstPersons.Count () > 0)
                    return Json (lstPersons);
                else
                    return NotFound ();
            } catch (Exception ex) {
                _loggerService.LogError (ex, ex.Message);
                return StatusCode (500, StringConstants.ProcessingError);
            }
        }
    }
}