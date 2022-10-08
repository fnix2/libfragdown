namespace libfragdown
{
    public readonly struct ImageMetaData
    {
        public ImageMetaData(ImageCoordinates imgCoordinates, Uri url)
        {
            Coordinates = imgCoordinates;
            Url = url;
        }
        public ImageCoordinates Coordinates { get; }

        public Uri Url { get; }
    }

    public readonly struct ImageCoordinates
    {
        public ImageCoordinates(int horizontal = 0, int vertical = 0)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        public int Horizontal { get; }

        public int Vertical { get; }

        public override string ToString()
        {
            return $"Hor: {Horizontal} Ver: {Vertical}";
        }
    }
}
