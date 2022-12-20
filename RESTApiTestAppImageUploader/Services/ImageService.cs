using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTApiTestAppImageUploader.Data;
using RESTApiTestAppImageUploader.Helpers;
using RESTApiTestAppImageUploader.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
using static RESTApiTestAppImageUploader.Helpers.ImageHelper;

namespace RESTApiTestAppImageUploader.Services
{
    public class ImageService : IImageService
    {
        private readonly TestAppDbContext _context;

        public ImageService(TestAppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Добавить изображение
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="directoryPath">Путь к локальному хранилищу</param>
        /// <returns></returns>
        public async Task<ImageUpload> AddImageAsync(IFormFile file, string directoryPath)
        {
            try
            {
                var fileFullPath = directoryPath + file.FileName;
                var fileName = System.IO.Path.GetFileNameWithoutExtension(fileFullPath);
                var imageType = ImageFormat.Jpeg;

                if (_context.ImageUploads.Any(x => x.ImageName == fileName))
                {
                    return null;
                }

                switch (ImageHelper.GetImageType(file))
                {
                    case ImageHelper.ImageType.Jpeg:
                        fileFullPath = directoryPath + fileName + ".png";
                        imageType = ImageFormat.Png;
                        break;
                    case ImageHelper.ImageType.Bmp:
                        fileFullPath = directoryPath + fileName + ".png";
                        imageType = ImageFormat.Png;
                        break;
                    case ImageHelper.ImageType.Png:
                        fileFullPath = directoryPath + fileName + ".bmp";
                        imageType = ImageFormat.Bmp;
                        break;
                }
                
                var transaction = _context.Database.BeginTransaction();
                var imageUpload = new ImageUpload
                {
                    ImageName = fileName,
                    ImagePath = fileFullPath,
                    ImageType = ImageHelper.ImageTypeConverter(imageType),
                };

                try
                {
                    _context.ImageUploads.Add(imageUpload);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return null;
                }
                
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    using (var image = Image.FromStream(memoryStream))
                    {
                        image.Save(fileFullPath, imageType);
                    }
                }
                
                return imageUpload;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Получить данные изображения в виде ImageUpload
        /// </summary>
        /// <param name="imageName">Название изображения</param>
        /// <returns></returns>
        public async Task<ImageUpload> GetImageAsync(string imageName)
        {
            if (!_context.ImageUploads.Any(x => x.ImageName == imageName))
            {
                return null;
            }

            var imageUpload = await _context.ImageUploads.FirstOrDefaultAsync(x => x.ImageName == imageName);

            return imageUpload;
        }

        /// <summary>
        /// Удалить изображение
        /// </summary>
        /// <param name="imageName">Название изображения</param>
        /// <returns></returns>
        public async Task<ImageUpload> DeleteImageAsync(string imageName)
        {
            if (!_context.ImageUploads.Any(x => x.ImageName == imageName))
            {
                return null;
            }

            var imageUpload = await _context.ImageUploads.FirstOrDefaultAsync(x => x.ImageName == imageName);

            var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.ImageUploads.Remove(imageUpload);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
 
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }

            System.IO.File.Delete(imageUpload.ImagePath);

            return imageUpload;
        }

        /// <summary>
        /// Послать изображение по адресу и получить ответ
        /// </summary>
        /// <param name="imagePath">Путь к изображению</param>
        /// <param name="directoryResponsePath">Путь к папке, где лежат файлы с результатом ответа запроса</param>
        /// <param name="address">Адрес</param>
        public async void SendImageTo(string imagePath, string directoryResponsePath, string address)
        {
            var imageBase64Format = ImageHelper.ConvertImageToBase64(imagePath);

            HttpClient client = new HttpClient();
            var postData = new PostData { Data = imageBase64Format };
            string contentData = JsonSerializer.Serialize(postData);
            var data = new StringContent(contentData);

            var response = await client.PostAsync(address, data);
            var statusCode = (int)response.StatusCode;
            var responseString = await response.Content.ReadAsStringAsync();

            if (!Directory.Exists(directoryResponsePath))
            {
                Directory.CreateDirectory(directoryResponsePath);
            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(directoryResponsePath, "ResponseData.txt")))
            {
                await outputFile.WriteAsync(responseString);
            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(directoryResponsePath, "ResponseStatusCode.txt")))
            {
                await outputFile.WriteAsync(statusCode.ToString());
            }
        }
    }
}
