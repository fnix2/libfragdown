using System.Collections;

namespace libfragdown
{
    public class CoordinatesGenerator : IEnumerable<ImageCoordinates>
    {
        public CoordinatesGenerator(ImageCoordinates maxImageCoordinates)
        {
            MaxImageCoordinates = maxImageCoordinates;
            _currentCoordinate = new(maxImageCoordinates);
        }

        public CoordinatesGenerator(ImageCoordinates maxImageCoordinates, int step) : this(maxImageCoordinates)
        {
            Step = step;
        }

        public ImageCoordinates Current => _currentCoordinate.Current;

        public int Count => (_currentCoordinate.MaxImageCoordinates.Horizontal + 1) *
            (_currentCoordinate.MaxImageCoordinates.Vertical + 1);
        public int Step { get; } = 1;


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
}
