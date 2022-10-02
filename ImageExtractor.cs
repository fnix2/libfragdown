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

    public class CoordinatesGenerator : IEnumerable<ImageCoordinates>
    {
        public CoordinatesGenerator(ImageCoordinates maxImageCoordinates)
        {
            MaxImageCoordinates = maxImageCoordinates;
            _currentCoordinate = new(maxImageCoordinates);
        }

        public CoordinatesGenerator(ImageCoordinates maxImageCoordinates, ImageCoordinates step) : this(maxImageCoordinates)
        {
            Step = step;
        }

        public ImageCoordinates Current => _currentCoordinate.Current;

        public int Count => (_currentCoordinate.MaxImageCoordinates.Horizontal + 1) *
            (_currentCoordinate.MaxImageCoordinates.Vertical + 1);

        public ImageCoordinates Step { get; } = new(1, 1);

        public ImageCoordinates MaxImageCoordinates { get; }

        private bool _currentNotInit = true;

        private SmartCoordinate _currentCoordinate;

        public IEnumerator<ImageCoordinates> GetEnumerator()
        {
            return new Iterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Iterator(this);
        }

        public void Dispose() { }

        //TODO logic move next must be more flexible
        public bool MoveNext()
        {
            if (_currentNotInit)
            {
                _currentNotInit = false;
                return true;
            }
            (_currentCoordinate, bool isSucces) = _currentCoordinate + Step;
            return isSucces;
        }

        public void Reset()
        {
            _currentCoordinate = new(MaxImageCoordinates);
            _currentNotInit = true;
        }

        private class Iterator : IEnumerator<ImageCoordinates>
        {
            public Iterator(CoordinatesGenerator corGen)
            {
                _coordinateGenerator = corGen;
            }

            public ImageCoordinates Current => _coordinateGenerator.Current;

            private readonly CoordinatesGenerator _coordinateGenerator;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                return _coordinateGenerator.MoveNext();

            }
            public void Reset()
            {
                _coordinateGenerator.Reset();
            }

            public void Dispose() { }
        }
    }

    public class UrlGenerator : IEnumerable<ImageMetaData>
    {
        private readonly CoordinatesGenerator _coordinatesGenerator;
        private readonly ConvertCoordinatesToUrl _convertToUrl;

        public UrlGenerator(CoordinatesGenerator corGen, ConvertCoordinatesToUrl convert)
        {
            _coordinatesGenerator = corGen;
            _convertToUrl = convert;
        }

        public IEnumerator<ImageMetaData> GetEnumerator()
        {
            return new Iterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Iterator(this);
        }

        private class Iterator : IEnumerator<ImageMetaData>
        {
            private readonly UrlGenerator _urlGenerator;

            public Iterator(UrlGenerator urlGen)
            {
                _urlGenerator = urlGen;
            }

            public ImageMetaData Current
            {
                get
                {
                    var currentImgCoor = _urlGenerator._coordinatesGenerator.Current;
                    return new ImageMetaData(currentImgCoor, _urlGenerator._convertToUrl.Convert(currentImgCoor));
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                return _urlGenerator._coordinatesGenerator.MoveNext();
            }

            public void Reset()
            {
                _urlGenerator._coordinatesGenerator.Reset();
            }
        }
    }

    public class FindMaxCoordinate
    {
        private readonly ConvertCoordinatesToUrl _convert;
        private static readonly HttpClient _httpClient = new();

        public FindMaxCoordinate(ConvertCoordinatesToUrl convert)
        {
            _convert = convert;
        }

        private enum CoordinateType
        {
            Horizontal,
            Vertical
        }

        public ImageCoordinates FindMaxCoordinates()
        {
            int horizontalMax = FindOneMaxCoordinate(CoordinateType.Horizontal);
            int verticalMax = FindOneMaxCoordinate(CoordinateType.Vertical);
            return new ImageCoordinates(horizontal: horizontalMax, vertical: verticalMax);
        }

        private int FindOneMaxCoordinate(CoordinateType coordinateType)
        {
            int left_border = 0;
            int right_border = 1;
            while (IsFileExist(_convert.Convert(CreateImageCoordinates(coordinateType, right_border))))
            {
                left_border = right_border;
                right_border *= 2;
            }
            while ((right_border - left_border) != 1)
            {
                int middle = left_border + ((right_border - left_border) / 2);
                if (IsFileExist(_convert.Convert(CreateImageCoordinates(coordinateType, middle))))
                {
                    left_border = middle;
                }
                else
                {
                    right_border = middle;
                }
            }
            return left_border;
        }

        private static bool IsFileExist(Uri url)
        {
            using HttpRequestMessage request = new(HttpMethod.Head, url); //TODO: Может застревать сдесь при проблемах с интернетом
            using HttpResponseMessage result = _httpClient.Send(request);
            return (int)result.StatusCode == 200;
        }

        private static ImageCoordinates CreateImageCoordinates(CoordinateType coordinateType, int coordinate)
        {
            return coordinateType == CoordinateType.Horizontal
                ? new ImageCoordinates(horizontal: coordinate)
                : new ImageCoordinates(vertical: coordinate);
        }
    }

    public abstract class ConvertCoordinatesToUrl
    {
        public ConvertCoordinatesToUrl(Uri uri)
        {
            MainUrl = uri;
        }

        protected virtual Uri MainUrl { get; }

        public abstract Uri Convert(ImageCoordinates imgCoordinate);
    }

    public abstract class ImageExtractor
    {
        public ImageExtractor(Uri url)
        {
            Url = url;
            ConvertToUrl = GetConvertToUrl(Url);
            MaxCoordinates = new FindMaxCoordinate(ConvertToUrl).FindMaxCoordinates();
            UrlGenerator = new(GetCoordinatesGenerator(), ConvertToUrl);
        }

        public abstract string Description { get; }

        public UrlGenerator UrlGenerator { get; private set; }


        public Uri Url { get; }

        public ImageCoordinates MaxCoordinates { get; private set; }

        protected ConvertCoordinatesToUrl ConvertToUrl { get; }

        public CoordinatesGenerator GetCoordinatesGenerator()
        {
            return new CoordinatesGenerator(MaxCoordinates);
        }

        protected abstract ConvertCoordinatesToUrl GetConvertToUrl(Uri url);
    }

    public class ImageExtractorGeneric : ImageExtractor
    {
        public ImageExtractorGeneric(Uri url) : base(url) { }

        public override string Description { get; } = "Generic image extractor module";

        protected override ConvertCoordinatesToUrl GetConvertToUrl(Uri url)
        {
            return new ConvertCoordinatesToUrlGeneric(url);
        }
    }

    public class ConvertCoordinatesToUrlGeneric : ConvertCoordinatesToUrl
    {
        public ConvertCoordinatesToUrlGeneric(Uri uri) : base(uri) { }

        public override Uri Convert(ImageCoordinates imgCoordinate)
        {
            string imgExtension = "jpg";
            string imageName = $"{imgCoordinate.Horizontal}_{imgCoordinate.Vertical}.{imgExtension}";
            return new Uri(MainUrl, imageName);
        }
    }
}
