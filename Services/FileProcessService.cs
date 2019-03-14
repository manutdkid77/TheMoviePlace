using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using TheMoviePlace;
using TheMoviePlace.Helpers;
using TheMoviePlace.Services;

namespace TheMoviePlace.Services {
    public class FileProcessService : IFileProcessService {
        IHostingEnvironment _hostingEnvironment;
        ILogger<FileProcessService> _loggerService;
        public FileProcessService (IHostingEnvironment hostingEnvironment,ILogger<FileProcessService> loggerService) {
            _hostingEnvironment = hostingEnvironment;
            _loggerService = loggerService;
        }

        public void ProcessFormFile (IFormFile formFile, ModelStateDictionary modelState) {
            try {
                if (!IsValidFileExtension (formFile, null)) {
                    modelState.AddModelError (formFile.Name,
                        StringConstants.FileNotValid);
                } else if (formFile.Length > 5000000) {
                    modelState.AddModelError (formFile.Name,
                        StringConstants.FileSizeExceeds);
                }
            } catch (Exception ex) {
                _loggerService.LogError(ex,ex.Message);
                modelState.AddModelError(formFile.Name,StringConstants.FileProcessingError);
            }
        }

        public string GetSanitizedFileName (string strFileName) {
            var sanitizedFileName = WebUtility.HtmlEncode (
                Path.GetFileName (strFileName));

            return sanitizedFileName;
        }

        public string GetFileExtension (string strFileName) {
            return Path.GetExtension (strFileName);
        }

        public async Task UploadFile (IFormFile filePoster,string strFileName, ModelStateDictionary modelState) {

            try {

                var filePath = Path.Combine (_hostingEnvironment.ContentRootPath, StringConstants.FileUploadFolder, strFileName);

                using (var stream = new FileStream (filePath, FileMode.Create)) {
                    await filePoster.CopyToAsync (stream);
                }
            } catch (Exception ex) {
                _loggerService.LogError(ex,ex.Message);
                modelState.AddModelError(filePoster.Name,StringConstants.FileProcessingError);
            }
        }

        private Dictionary<string, List<byte[]>> fileSignature = new Dictionary<string, List<byte[]>> { { ".DOC", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
            { ".DOCX", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
            { ".PDF", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
            {
            ".ZIP",
            new List<byte[]> {
            new byte[] { 0x50, 0x4B, 0x03, 0x04 },
            new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x55 },
            new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 },
            new byte[] { 0x50, 0x4B, 0x05, 0x06 },
            new byte[] { 0x50, 0x4B, 0x07, 0x08 },
            new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 }
            }
            },
            { ".PNG", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
            {
            ".JPG",
            new List<byte[]> {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
            }
            },
            {
            ".JPEG",
            new List<byte[]> {
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
            new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
            }
            },
            {
            ".XLS",
            new List<byte[]> {
            new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 },
            new byte[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 },
            new byte[] { 0xFD, 0xFF, 0xFF, 0xFF }
            }
            },
            { ".XLSX", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
            { ".GIF", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } }
        };

        private bool IsValidFileExtension (IFormFile formFile, byte[] allowedChars) {
            if (formFile == null)
                return false;

            byte[] fileData;

            using (var ms = new MemoryStream ()) {
                formFile.CopyTo (ms);
                fileData = ms.ToArray ();
            }

            string fileName = GetSanitizedFileName (formFile.FileName);

            if (string.IsNullOrEmpty (fileName) || fileData == null || fileData.Length == 0) {
                return false;
            }

            bool flag = false;
            string ext = GetFileExtension (fileName);
            if (string.IsNullOrEmpty (ext)) {
                return false;
            }

            ext = ext.ToUpperInvariant ();

            if (ext.Equals (".TXT") || ext.Equals (".CSV") || ext.Equals (".PRN")) {
                foreach (byte b in fileData) {
                    if (b > 0x7F) {
                        if (allowedChars != null) {
                            if (!allowedChars.Contains (b)) {
                                return false;
                            }
                        } else {
                            return false;
                        }
                    }
                }

                return true;
            }

            if (!fileSignature.ContainsKey (ext)) {
                return true;
            }

            List<byte[]> sig = fileSignature[ext];
            foreach (byte[] b in sig) {
                var curFileSig = new byte[b.Length];
                Array.Copy (fileData, curFileSig, b.Length);
                if (curFileSig.SequenceEqual (b)) {
                    flag = true;
                    break;
                }
            }

            return flag;
        }
    }
}