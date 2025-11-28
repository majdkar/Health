using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SchoolV01.Shared;
using SchoolV01.Shared.Constants;

namespace SchoolV01.Api.Controllers
{
    public class FileUploadController : ApiControllerBase
    {
        private readonly IWebHostEnvironment env;

        public FileUploadController(IWebHostEnvironment env)
        {
            this.env=env;
        }

        public class UploadFileRequest
        {
            [FromForm]
            public IFormFile File { get; set; }
        }

        [HttpPost("{fileLocation:int}/{uploadType:int}")]
        public async Task<IActionResult> Upload(
            [FromForm] UploadFileRequest request,
            int fileLocation,
            int uploadType)
        {
            var file = request.File;

            if (file == null)
                return BadRequest("File not provided.");

            try
            {
                var requestUploadType = (Enums.UploadFileTypeEnum)uploadType;

                if (!CheckFileExtension(file, requestUploadType))
                {
                    return BadRequest("Invalid file extension.");
                }

                if (requestUploadType != Enums.UploadFileTypeEnum.Video)
                {
                    if (!CheckFileSize(file))
                        return BadRequest("File exceeds 10 MB.");
                }
                else
                {
                    if (!CheckVideoFileSize(file))
                        return BadRequest("Video exceeds 500 MB.");
                }

                var folder = Path.Combine(Constants.UploadFolderName, ((Enums.FileLocation)fileLocation).ToString());
                Directory.CreateDirectory(folder);

                var ext = Path.GetExtension(file.FileName);
                var newFileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(folder, newFileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }

                return Ok(newFileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        /// The file extension must be jpg/bmp/png for images and txt/docx/pdf for files

        private bool CheckFileExtension(IFormFile file, Enums.UploadFileTypeEnum type)
        {
            var fileNameExtension = file.FileName.Split(".").LastOrDefault();

            string[] extensions = new string[] { };
            if(type == Enums.UploadFileTypeEnum.Image)
                extensions = new string[] { "jpg", "bmp", "png","jpeg","ico" };
            if (type == Enums.UploadFileTypeEnum.Chart)
                extensions = new string[] { "jpg", "bmp", "png", "jpeg" };
            if (type == Enums.UploadFileTypeEnum.File)
                extensions = new string[] { "txt", "docx", "pdf" ,"xlsx" };
            if (type == Enums.UploadFileTypeEnum.Brochure)
                extensions = new string[] { "txt", "docx", "pdf", "xlsx" };
            if (type == Enums.UploadFileTypeEnum.Video)
                extensions = new string[] { "avi", "mp4", "mpeg" };

            if (string.IsNullOrEmpty(fileNameExtension) ||
                !extensions.Contains(fileNameExtension.ToLower()))
            {
                return false;
            }

            return true;
        }

        /// Check the file size, it must be less than 10 mb

        private bool CheckFileSize(IFormFile file)
        {
            if (file.Length > 1e+7)
            {
                return false;
            }
            return true;
        }
        private bool CheckVideoFileSize(IFormFile file)
        {
            if (file.Length > 5e+7)
            {
                return false;
            }
            return true;
        }
    }
}