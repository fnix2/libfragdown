using System.Text;

namespace libfragdown
{
    public class ImageStorage
    {
        private readonly string _pathToImagesDirectory;

        private readonly string _montagedImageName;

        public ImageStorage(Uri baseUrl, string imageDirParentPath = "/home/tihon/tmp/libfragdown_images/", string montagedImageName = "Montaged.jpg")
        {
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            var urlHash = Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(baseUrl.ToString())));
            string fullDirPath = Path.Combine(imageDirParentPath, urlHash);
            _pathToImagesDirectory = fullDirPath;
            _montagedImageName = montagedImageName;
            Directory.CreateDirectory(fullDirPath);
        }

        public string GetMontagedImagePath()
        {
            return Path.Combine(_pathToImagesDirectory, _montagedImageName);
        }

        public string GetImagePath(ImageCoordinates coordinates)
        {
            string imageName = $"{coordinates.Vertical}_{coordinates.Horizontal}";
            return Path.Combine(_pathToImagesDirectory, imageName);
        }
    }
}
