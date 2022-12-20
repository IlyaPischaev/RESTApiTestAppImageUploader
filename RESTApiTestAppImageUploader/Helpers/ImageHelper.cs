using System.Drawing.Imaging;

namespace RESTApiTestAppImageUploader.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Тип изображения
        /// </summary>
        public enum ImageType
        {
            Unknown,
            Jpeg,
            Png,
            Bmp
        }

        /// <summary>
        /// Узнать тип файла
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns></returns>
        public static ImageType GetImageType(IFormFile file)
        {
            switch (file.ContentType.ToUpperInvariant())
            {
                case "IMAGE/JPEG":
                    return ImageType.Jpeg;
                case "IMAGE/PNG":
                    return ImageType.Png;
                case "IMAGE/BMP":
                    return ImageType.Bmp;
            }

            return ImageType.Unknown;
        }

        /// <summary>
        /// Получить тип изображения в удобном виде для System
        /// </summary>
        /// <param name="imageType">Тип изображения</param>
        /// <returns></returns>
        public static string ImageTypeConverter(ImageFormat imageType)
        {
            if (imageType == ImageFormat.Jpeg)
            {
                return "image/jpeg";
            }
            else if (imageType == ImageFormat.Png)
            {
                return "image/png";
            } else if (imageType == ImageFormat.Bmp)
            {
                return "image/bmp";
            }

            return "unknown";
        }

        /// <summary>
        /// Преобразовать изображение в base64 формат
        /// </summary>
        /// <param name="imagePath">Путь к изображению</param>
        /// <returns></returns>
        public static string ConvertImageToBase64(string imagePath)
        {
            byte[] imageArray = File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(imageArray);
        }
    }
}
