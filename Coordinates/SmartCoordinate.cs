namespace libfragdown
{
    public struct SmartCoordinate
    {
        public SmartCoordinate(ImageCoordinates maxImageCoordinates, ImageCoordinates current = new())
        {
            MaxImageCoordinates = maxImageCoordinates;
            Current = current;
        }

        public static (SmartCoordinate newValue, bool isSucces) operator +(SmartCoordinate a, ImageCoordinates b)
        {
            return a.Add(b);
        }

        public ImageCoordinates Current { get; }

        public ImageCoordinates MaxImageCoordinates { get; }

        public (SmartCoordinate newValue, bool isSucces) Add(ImageCoordinates addCoordinate)
        {
            var curVer = Current.Vertical;
            var curHor = Current.Horizontal;
            var addVer = addCoordinate.Vertical;
            var addHor = addCoordinate.Horizontal;
            if ((curHor + addHor) > MaxImageCoordinates.Horizontal)
            {
                if ((curVer + addVer) > MaxImageCoordinates.Vertical)
                {
                    return (new(MaxImageCoordinates, Current), false);
                }
                else
                {
                    var newCurrent = new ImageCoordinates(vertical: curVer + addVer);
                    return (new(MaxImageCoordinates, newCurrent), true);
                }
            }
            else
            {
                var newCurrent = new ImageCoordinates(horizontal: curHor + addHor, vertical: curVer);
                return (new(MaxImageCoordinates, newCurrent), true);
            }
        }
    }
}
