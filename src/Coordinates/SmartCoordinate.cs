namespace libfragdown
{
    public readonly struct SmartCoordinate
    {
        public SmartCoordinate(ImageCoordinates maxImageCoordinates, ImageCoordinates current = new())
        {
            MaxImageCoordinates = maxImageCoordinates;
            Current = current;
        }

        public static (SmartCoordinate newValue, bool isSucces) operator +(SmartCoordinate a, int b)
        {
            return a.Add(b);
        }

        public ImageCoordinates Current { get; }

        public ImageCoordinates MaxImageCoordinates { get; }

        public (SmartCoordinate newValue, bool isSucces) Add(int add)
        {
            int curNumber = GetNumberFromCoordinate(Current);
            int maxNumber = GetNumberFromCoordinate(MaxImageCoordinates);
            if (maxNumber >= add + curNumber)
            {
                return new(new(MaxImageCoordinates, GetCoordinateFromNumber(add + curNumber)), true);
            }
            return new(new(MaxImageCoordinates, Current), false);
        }

        private ImageCoordinates GetCoordinateFromNumber(int number)
        {
            int hor = ((number % (MaxImageCoordinates.Horizontal + 1)) +
                    MaxImageCoordinates.Horizontal) % (MaxImageCoordinates.Horizontal + 1);
            int ver = (int)Math.Ceiling((double)number / (MaxImageCoordinates.Horizontal + 1)) - 1;
            return new(horizontal: hor, vertical: ver);
        }

        private int GetNumberFromCoordinate(ImageCoordinates coordinates)
        {
            return (coordinates.Vertical * (MaxImageCoordinates.Horizontal + 1)) + coordinates.Horizontal + 1;
        }
    }
}
