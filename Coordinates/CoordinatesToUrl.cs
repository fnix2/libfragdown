namespace libfragdown
{
    public interface ICoordinatesToUrl
    {
        public Uri Convert(ImageCoordinates imgCoordinate);
    }

    public class CoordinatesToUrlGeneric : ICoordinatesToUrl
    {
        private readonly Uri _mainUrl;
        private const string _imgExtension = "jpg";

        public CoordinatesToUrlGeneric(Uri uri)
        {
            _mainUrl = uri;
        }

        public Uri Convert(ImageCoordinates imgCoordinate)
        {
            string imageName = $"{imgCoordinate.Horizontal}_{imgCoordinate.Vertical}.{_imgExtension}";
            return new Uri(_mainUrl, imageName);
        }
    }
}
