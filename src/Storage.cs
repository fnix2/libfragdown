using System.Text;

namespace libfragdown
{
    public class ImageStorage
    {
        private readonly string _pathToImagesDirectory;

        private readonly string _pathToMontagedImage;

        public ImageStorage(Uri baseUrl, string pathToImagesDirectory, string pathToMontagedImage)
        {
            string urlHash = GetHashedUrl(baseUrl);
            _pathToImagesDirectory = Path.Combine(pathToImagesDirectory, "libfragdown", urlHash);
            _pathToMontagedImage = pathToMontagedImage;
            Directory.CreateDirectory(_pathToImagesDirectory);
        }

        public ImageStorage(Uri baseUrl)
        {
            string urlHash = GetHashedUrl(baseUrl);
            _pathToImagesDirectory = Path.Combine(Path.GetTempPath(), "libfragdown", urlHash);
            _pathToMontagedImage = Path.Combine(_pathToImagesDirectory, "Montaged.jpg");
            Directory.CreateDirectory(_pathToImagesDirectory);
        }

        public string GetMontagedImagePath()
        {
            return Path.Combine(_pathToImagesDirectory, _pathToMontagedImage);
        }

        public string GetImagePath(ImageCoordinates coordinates)
        {
            string imageName = $"{coordinates.Vertical}_{coordinates.Horizontal}";
            return Path.Combine(_pathToImagesDirectory, imageName);
        }

        private static string GetHashedUrl(Uri url)
        {
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            var urlHash = Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(url.ToString())));
            return urlHash;
        }
    }
}
