using System.Collections;

namespace libfragdown
{
    public struct ImageMetaData
    {
        public ImageMetaData(ImageCoordinates imgCoordinates, Uri url)
        {
            Coordinates = imgCoordinates;
            Url = url;
        }
        public ImageCoordinates Coordinates { get; private set; }

        public Uri Url { get; private set; }
    }

    public struct ImageCoordinates
    {
        public ImageCoordinates(int horizontal = 0, int vertical = 0)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        public int Horizontal { get; private set; }

        public int Vertical { get; private set; }

        public override string ToString()
        {
            return $"Ver: {Vertical} Hor: {Horizontal}";
        }

        public (int ver, int hor) GetCorrdinates()
        {
            return (ver: Vertical, hor: Horizontal);
        }
    }
}
