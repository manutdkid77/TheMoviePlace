using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheMoviePlace.Entities;
using TheMoviePlace.Helpers;
using TheMoviePlace.Models;

namespace TheMoviePlace.Controllers
{
    public class PersonController : Controller
    {
        TheMoviePlaceDBContext _TheMoviePlaceDBContext;
        ILogger<PersonController> _loggerService;
        public PersonController(TheMoviePlaceDBContext TheMoviePlaceDBContext, ILogger<PersonController> loggerService)
        {
            _TheMoviePlaceDBContext = TheMoviePlaceDBContext;
            _loggerService = loggerService;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddPerson([Bind("Name", "Bio", "DateOfBirth", "GenderReferenceID","MovieID","RoleReferenceID")]AddPersonDTO oAddPersonDTO)
        {
            try
            {
                if (oAddPersonDTO is null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var oPerson = new Person(){
                    Name = oAddPersonDTO.Name,
                    Bio = oAddPersonDTO.Bio,
                    DateOfBirth = oAddPersonDTO.DateOfBirth,
                    GenderReferenceID = oAddPersonDTO.GenderReferenceID
                };

                await _TheMoviePlaceDBContext.Persons.AddAsync(oPerson);
                await _TheMoviePlaceDBContext.SaveChangesAsync();
                
                if(oAddPersonDTO.MovieID != 0){
                    var bAddResult = await AddRoleToMovieAsync(oAddPersonDTO.MovieID,oPerson.PersonID,oAddPersonDTO.RoleReferenceID);
                    
                    if(!bAddResult)
                        return NotFound();
                }

                return Ok(oPerson);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, ex.Message);
                return StatusCode(500, StringConstants.ProcessingError);
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchPerson(string Name)
        {
            try
            {

                if (string.IsNullOrEmpty(Name) || string.IsNullOrWhiteSpace(Name))
                    return BadRequest("Name is required");

                var lstPersons = await _TheMoviePlaceDBContext.Persons.Where(p => p.Name.Contains(Name)).OrderBy(p => p.Name).ToListAsync();

                if (lstPersons.Count() > 0)
                    return Json(lstPersons);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, ex.Message);
                return StatusCode(500, StringConstants.ProcessingError);
            }
        }

        private async Task<bool> AddRoleToMovieAsync(int iMovieID, int iPersonID, int iRoleReferenceID){
            var oMovie = await _TheMoviePlaceDBContext.Roles.FirstOrDefaultAsync (r => r.MovieID == iMovieID);

            if (oMovie is null)
                return false;

            await _TheMoviePlaceDBContext.Roles.AddAsync (new Role () {
                MovieID = iMovieID,
                    PersonID = iPersonID,
                    RoleReferenceID = iRoleReferenceID
            });

            await _TheMoviePlaceDBContext.SaveChangesAsync ();

            return true;
        }
    }
}