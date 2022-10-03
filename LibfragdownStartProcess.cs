namespace libfragdown
{
    public enum LibfragdownProcessStates
    {
        NotInit,
        Searching,
        ReadyForDownload,
        Downloading,
        Montaging,
        Complete,
        Error,
    }

    public class LibfragdownStartProcess
    {
        public LibfragdownStartProcess(Uri url, ICoordinatesToUrl coordinatesToUrl,
                IRemoteCoordinateState remoteCoordinateState)
        {
            Url = url;
            CoordinatesToUrl = coordinatesToUrl;
            State = LibfragdownProcessStates.Searching;
            //TODO check if can find Coordinates
            MaxCoordinates = MaxCoordinatesSearch.FindMaxCoordinates(remoteCoordinateState);
            UrlGenerator = new(new CoordinatesGenerator(MaxCoordinates), CoordinatesToUrl);
            State = LibfragdownProcessStates.ReadyForDownload;
        }

        public LibfragdownStartProcess(Uri url, ICoordinatesToUrl coordinatesToUrl)
            : this(url, coordinatesToUrl, new RemoteCoordinateStateGeneric(coordinatesToUrl))
        { }

        public LibfragdownStartProcess(Uri url, IRemoteCoordinateState coordinateState)
            : this(url, new CoordinatesToUrlGeneric(url), coordinateState)
        { }

        public LibfragdownStartProcess(Uri url)
            : this(url, new CoordinatesToUrlGeneric(url))
        { }

        public UrlGenerator UrlGenerator { get; }

        public LibfragdownProcessStates State { get; private set; }

        public Uri Url { get; }

        public ImageCoordinates MaxCoordinates { get; }

        public void Start(bool deleteDownloadedFiles=true)
        {
            State = LibfragdownProcessStates.Downloading;
            ImageStorage storage = new(Url);
            Downloader downloader = new(UrlGenerator, storage);
            bool downloadSuccess = downloader.DownloadImages();
            if (!downloadSuccess)
            {
                State = LibfragdownProcessStates.Error;
                return;
            }

            State = LibfragdownProcessStates.Montaging;
            ImageMontage montage = new(new CoordinatesGenerator(MaxCoordinates), storage);
            bool montageSuccess = montage.Montage(deleteDownloadedFiles);

            if (!montageSuccess)
            {
                State = LibfragdownProcessStates.Error;
                return;
            }
            State = LibfragdownProcessStates.Complete;
        }

        private ICoordinatesToUrl CoordinatesToUrl { get; }
    }
}
