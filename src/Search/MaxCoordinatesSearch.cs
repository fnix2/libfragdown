namespace libfragdown
{
    public static class MaxCoordinatesSearch
    {
        private static IRemoteCoordinateState? _coordinateExist;

        private enum CoordinateType
        {
            Horizontal,
            Vertical
        }

        public static ImageCoordinates FindMaxCoordinates(IRemoteCoordinateState remoteCoordinateState)
        {
            _coordinateExist = remoteCoordinateState;
            int horizontalMax = FindOneMaxCoordinate(CoordinateType.Horizontal);
            int verticalMax = FindOneMaxCoordinate(CoordinateType.Vertical);
            return new ImageCoordinates(horizontal: horizontalMax, vertical: verticalMax);
        }

        private static int FindOneMaxCoordinate(CoordinateType coordinateType)
        {
            //TODO prevent endless cycle
            int leftBorder = 0;
            int rightBorder = 1;
            while (_coordinateExist.IsCoordinateExist(CreateImageCoordinates(coordinateType, rightBorder)))
            {
                leftBorder = rightBorder;
                rightBorder *= 2;
            }
            while ((rightBorder - leftBorder) != 1)
            {
                int middle = leftBorder + ((rightBorder - leftBorder) / 2);
                if (_coordinateExist.IsCoordinateExist(CreateImageCoordinates(coordinateType, middle)))
                {
                    leftBorder = middle;
                }
                else
                {
                    rightBorder = middle;
                }
            }
            return leftBorder;
        }

        private static ImageCoordinates CreateImageCoordinates(CoordinateType coordinateType, int coordinate)
        {
            return coordinateType == CoordinateType.Horizontal
                ? new ImageCoordinates(horizontal: coordinate)
                : new ImageCoordinates(vertical: coordinate);
        }
    }
}
