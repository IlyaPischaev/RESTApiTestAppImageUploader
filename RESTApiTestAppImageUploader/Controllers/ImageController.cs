using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTApiTestAppImageUploader.Data;
using RESTApiTestAppImageUploader.Helpers;
using RESTApiTestAppImageUploader.Models;
using RESTApiTestAppImageUploader.Services;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;

namespace RESTApiTestAppImageUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageService _imageService;

        public ImageController(IWebHostEnvironment webHostEnvironment, IImageService imageService)
        {
            _webHostEnvironment = webHostEnvironment;
            _imageService = imageService;
        }

        /// <summary>
        /// Добавить изображение
        /// </summary>
        /// <param name="file">Файл с изображением</param>
        /// <returns></returns>
        [HttpPost("AddImage")]
        public async Task<IActionResult> UploadImageAsync(IFormFile file)
        {
            var directoryPath = _webHostEnvironment.WebRootPath + "\\Images\\";
            var directoryResponsePath = _webHostEnvironment.WebRootPath + "\\Responses\\";
            var address = "https://httpbin.org/post";

            try
            {
                if (file.Length > 0)
                {
                    if (ImageHelper.GetImageType(file) == ImageHelper.ImageType.Unknown)
                    {
                        return BadRequest("Unknown image type, failed to load");
                    }

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var imageUpload = await _imageService.AddImageAsync(file, directoryPath);
                    if (imageUpload == null) return BadRequest();

                    _imageService.SendImageTo(imageUpload.ImagePath, directoryResponsePath, address);

                    return Ok(imageUpload);
                }
                else
                {
                    return BadRequest("Failed to load image");
                }
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Получить данные изображения
        /// </summary>
        /// <param name="imageName">Название изображение</param>
        /// <returns></returns>
        [HttpGet("GetImage")]
        public async Task<ActionResult<Image>> GetImageAsync(string imageName)
        {
            var imageUpload = await _imageService.GetImageAsync(imageName);
            if (imageUpload == null) return NotFound();
            
            var image = Image.FromFile(imageUpload.ImagePath);

            return Ok(image);
        }

        /// <summary>
        /// Получить изображение
        /// </summary>
        /// <param name="imageName">Название изображения</param>
        /// <returns></returns>
        [HttpGet("GetImageAsPicture")]
        public async Task<IActionResult> GetImageAsPictureAsync(string imageName)
        {
            var imageUpload = await _imageService.GetImageAsync(imageName);
            if (imageUpload == null) return NotFound();

            var stream = System.IO.File.OpenRead(imageUpload.ImagePath);
            var file = new FileStreamResult(stream, imageUpload.ImageType);

            return file;    
        }

        /// <summary>
        /// Удалить изображение
        /// </summary>
        /// <param name="imageName">Название изображения</param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<ActionResult<ImageUpload>> DeleteImageAsync(string imageName)
        {
            var imageUpload = await _imageService.DeleteImageAsync(imageName);
            if (imageUpload == null) return NotFound();

            return Ok(imageUpload);
        }
    }
}
