using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TheMoviePlace.Services {
    public interface IFileProcessService {
        void ProcessFormFile (IFormFile formFile, ModelStateDictionary modelState);
        string GetSanitizedFileName (string strFileName);
        string GetFileExtension (string strFileName);
        Task UploadFile (IFormFile filePoster,string strFileName, ModelStateDictionary modelState);
    }
}