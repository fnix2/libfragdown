using NetVips;
namespace libfragdown
{
    public class ImageMontage
    {
        private readonly ImageStorage _imageStorage;
        private readonly CoordinatesGenerator _coordinatesGenerator;

        public ImageMontage(CoordinatesGenerator coordinatesGenerator, ImageStorage imageStorage)
        {
            _coordinatesGenerator = coordinatesGenerator;
            _imageStorage = imageStorage;
        }

        public void Montage(bool deleteTiles = true)
        {
            // One thread use less ram and working faster
            NetVips.NetVips.Concurrency = 1;
            /* Environment.SetEnvironmentVariable("VIPS_DISC_THRESHOLD", "1m"); */
            Image[] images = new Image[_coordinatesGenerator.Count];

            int i = 0;
            foreach (var coordinate in _coordinatesGenerator)
            {
                images[i] = Image.NewFromFile(_imageStorage.GetImagePath(coordinate));
                i++;
            }

            using var image = Image.Arrayjoin(images, _coordinatesGenerator.MaxImageCoordinates.Horizontal + 1);

            Console.WriteLine();
            foreach (var im in images)
            {
                im.Dispose();
            }

            image.WriteToFile(_imageStorage.GetMontagedImagePath());
            if (deleteTiles)
            {
                _coordinatesGenerator.Reset();
                foreach (var coordinate in _coordinatesGenerator)
                {
                    File.Delete(_imageStorage.GetImagePath(coordinate));
                }
            }
        }
    }
}
