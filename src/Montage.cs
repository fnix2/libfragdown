using NetVips;
namespace libfragdown
{
    public class Montager
    {
        private readonly ImageStorage _imageStorage;
        private CoordinatesGenerator? _coordinatesGenerator;

        public Montager(ImageStorage imageStorage)
        {
            _imageStorage = imageStorage;
        }

        public bool StartMontage(CoordinatesGenerator coordinatesGenerator, bool deleteTiles = true)
        {
            _coordinatesGenerator = coordinatesGenerator;
            // For joining many images more then one thread don't icrease processing speed, but use more ram 
            NetVips.NetVips.Concurrency = 1;
            Image[] imageChunksGrid = new Image[_coordinatesGenerator.Count];

            int i = 0;
            int widthRes = 0;
            int heightRes = 0;
            foreach (var coordinate in _coordinatesGenerator)
            {
                imageChunksGrid[i] = Image.NewFromFile(_imageStorage.GetImagePath(coordinate));
                widthRes += imageChunksGrid[i].Width;
                heightRes += imageChunksGrid[i].Height;
                i++;
            }
            int imageChunksGridWidth = _coordinatesGenerator.MaxImageCoordinates.Horizontal + 1;
            int imageChunksGridHeight = _coordinatesGenerator.MaxImageCoordinates.Vertical + 1;
            widthRes /= imageChunksGridHeight;
            heightRes /= imageChunksGridWidth;

            using Image image = Image.Arrayjoin(imageChunksGrid, imageChunksGridWidth).Crop(0, 0, widthRes, heightRes);

            image.WriteToFile(_imageStorage.GetMontagedImagePath());
            if (deleteTiles)
            {
                _coordinatesGenerator.Reset();
                foreach (var coordinate in _coordinatesGenerator)
                {
                    File.Delete(_imageStorage.GetImagePath(coordinate));
                }
            }
            //TODO check if Montage complete
            return true;
        }
    }
}
