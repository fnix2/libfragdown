namespace libfragdown
{
    public class Downloader
    {
        private UrlGenerator _urlGenerator;
        private readonly ImageStorage _imageStorage;
        private readonly IUrlToStream _urlToStream;

        public Downloader(ImageStorage imageStorage, IUrlToStream urlToStream)
        {
            _imageStorage = imageStorage;
            _urlToStream = urlToStream;
        }

        public bool DownloadImages(UrlGenerator urlGenerator)
        {
            _urlGenerator = urlGenerator;
            foreach (var url in _urlGenerator)
            {
                DownloadImage(url);
            }
            //TODO check if Download complete
            return true;
        }

        //TODO move to utils class?
        private static bool IsFileImage(FileStream fileStream)
        {
            List<byte[]> imageHeaderPatterns = new()
            {
                    new byte[] { 0x42, 0x4D },               // BMP "BM"
                    new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 },     // "GIF87a"
                    new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 },     // "GIF89a"
                    new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },   // PNG "\x89PNG\x0D\0xA\0x1A\0x0A"
                    new byte[] { 0x49, 0x49, 0x2A, 0x00 }, // TIFF II "II\x2A\x00"
                    new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, // TIFF MM "MM\x00\x2A"
                    new byte[] { 0xFF, 0xD8, 0xFF },        // JPEG JFIF (SOI "\xFF\xD8" and half next marker xFF)
            };

            byte[] fileHeader;

            using (BinaryReader binaryReader = new(fileStream))
            {
                fileHeader = binaryReader.ReadBytes(8);
            }

            foreach (var pattern in imageHeaderPatterns)
            {
                Range range = 0..pattern.Length;

                if (fileHeader.Length < pattern.Length)
                {
                    continue;
                }

                if (fileHeader[range].SequenceEqual(pattern))
                {
                    return true;
                }
            }
            return false;
        }

        private void DownloadImage(ImageMetaData url)
        {
            string imagePath = _imageStorage.GetImagePath(url.Coordinates);

            if (File.Exists(imagePath))
            {
                Console.WriteLine($"File {imagePath} is exist!");
                return;
            }

            string tempFileName = Path.GetTempFileName();
            using FileStream imageFile = File.Create(tempFileName);

            Stream downloadStream = _urlToStream.GetStream(url.Url);
            using (Task task = downloadStream.CopyToAsync(imageFile))
            {
                task.Wait();
            }

            //TODO move position set to IsFileImage?
            imageFile.Position = 0;
            if (IsFileImage(imageFile))
            {
                File.Move(tempFileName, imagePath);
                Console.WriteLine($"File {imagePath} writen");
            }
            else
            {
                File.Delete(tempFileName);
                Console.WriteLine($"File {imagePath} not image!");
            }
        }
    }
}
