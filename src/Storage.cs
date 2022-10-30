using System.Text;

namespace libfragdown
{
    public class ImageStorage
    {
        private const string _defaultImageName = "Montaged.jpg";

        private const string _defaultTilesDirectoryPrefix = "libfragdown";

        private readonly string _pathToTilesDirectory;

        private readonly string _pathToMontagedImage;

        public ImageStorage(Uri baseUrl, string pathToTilesDirectory, string pathToMontagedImage)
        {
            string urlHash = GetHashedUrl(baseUrl);
            _pathToTilesDirectory = Path.Combine(pathToTilesDirectory, _defaultTilesDirectoryPrefix, urlHash);
            _pathToMontagedImage = pathToMontagedImage;
            Directory.CreateDirectory(_pathToTilesDirectory);
        }

        public ImageStorage(Uri baseUrl)
        {
            string urlHash = GetHashedUrl(baseUrl);
            _pathToTilesDirectory = Path.Combine(Path.GetTempPath(), _defaultTilesDirectoryPrefix, urlHash);
            _pathToMontagedImage = Path.Combine(_pathToTilesDirectory, _defaultImageName);
            Directory.CreateDirectory(_pathToTilesDirectory);
        }

        public ImageStorage(Uri baseUrl, string pathToMontagedImage)
        {
            string urlHash = GetHashedUrl(baseUrl);
            _pathToTilesDirectory = Path.Combine(Path.GetTempPath(), _defaultTilesDirectoryPrefix, urlHash);
            _pathToMontagedImage = pathToMontagedImage;
            Directory.CreateDirectory(_pathToTilesDirectory);
        }

        public string GetMontagedImagePath()
        {
            return Path.Combine(_pathToMontagedImage);
        }

        public string GetImagePath(ImageCoordinates coordinates)
        {
            string imageName = $"{coordinates.Horizontal}_{coordinates.Vertical}";
            return Path.Combine(_pathToTilesDirectory, imageName);
        }

        private static string GetHashedUrl(Uri url)
        {
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            var urlHash = Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(url.ToString())));
            return urlHash;
        }
    }
}
