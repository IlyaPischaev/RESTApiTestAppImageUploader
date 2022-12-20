using Microsoft.AspNetCore.Mvc;
using RESTApiTestAppImageUploader.Models;
using System.Drawing;

namespace RESTApiTestAppImageUploader.Services
{
    public interface IImageService
    {
        Task<ImageUpload> AddImageAsync(IFormFile file, string directoryPath);
        Task<ImageUpload> GetImageAsync(string imageName);
        Task<ImageUpload> DeleteImageAsync(string imageName);
        void SendImageTo(string imagePath, string directoryResponsePath, string address);
    }
}
