namespace libfragdown
{
    public class UrlGenerator : IEnumerable<ImageMetaData>
    {
        private readonly CoordinatesGenerator _coordinatesGenerator;
        private readonly ICoordinatesToUrl _convertToUrl;

        public UrlGenerator(CoordinatesGenerator corGen, ICoordinatesToUrl convert)
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
}
