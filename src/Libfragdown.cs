namespace libfragdown;

public enum LibfragdownHandlersType
{
    Generic,
    CloserToVanEyck
}

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

public class LibfragdownState
{
}

public static class Libfragdown
{
    public static LibfragdownState LibfragdownStart(Uri url, LibfragdownHandlersType handlerType)
    {
        LibfragdownState state = new();
        if (handlerType == LibfragdownHandlersType.Generic)
        {
            LibfragdownStart(url, new HandlerParametersGeneric(), state);
        }
        else if (handlerType == LibfragdownHandlersType.CloserToVanEyck)
        {
            LibfragdownStart(url, new HandlerParametersCloserToVanEyck(), state);
        }
        return state;
    }

    public static void LibfragdownStart(Uri url, HandlerParametersGeneric options, LibfragdownState state)
    {
        ICoordinatesToUrl coordinatesToUrl = options.CoordinatesToUrl(url);
        IRemoteCoordinateState remoteCoordinateState = options.RemoteCoordinateState(coordinatesToUrl);
        MaxCoordinatesSearch maxCoordinatesSearch = options.MaxCoordinatesSearch(remoteCoordinateState);
        ImageCoordinates maxCoordinates = maxCoordinatesSearch.FindMaxCoordinates();
        CoordinatesGenerator coordinatesGenerator = options.CoordinatesGenerator(maxCoordinates);
        UrlGenerator urlGenerator = options.UrlGenerator(coordinatesGenerator, coordinatesToUrl);
        ImageStorage imageStorage = options.ImageStorage(url);
        IUrlToStream urlToStream = options.UrlToStream();
        Downloader downloader = options.Downloader(imageStorage, urlToStream);
        downloader.DownloadImages(urlGenerator);
        ImageMontage imageMontage = options.ImageMontage(imageStorage);
        coordinatesGenerator.Reset();
        imageMontage.Montage(coordinatesGenerator);
    }
}

public class HandlerParametersGeneric
{
    public virtual ICoordinatesToUrl CoordinatesToUrl(Uri url)
    {
        return new CoordinatesToUrlGeneric(url);
    }

    public virtual IRemoteCoordinateState RemoteCoordinateState(ICoordinatesToUrl coordinatesToUrl)
    {
        return new RemoteCoordinateStateGeneric(coordinatesToUrl);
    }

    public virtual MaxCoordinatesSearch MaxCoordinatesSearch(IRemoteCoordinateState remoteCoordinateState)
    {
        return new MaxCoordinatesSearch(remoteCoordinateState);
    }

    public virtual ImageStorage ImageStorage(Uri url)
    {
        return new ImageStorage(url);
    }

    public virtual ImageStorage ImageStorage(Uri url, string pathToImagesDirectory, string pathToMontagedImage)
    {
        return new ImageStorage(url, pathToImagesDirectory, pathToMontagedImage);
    }

    public virtual IUrlToStream UrlToStream()
    {
        return new UrlToStreamGeneric();
    }

    public virtual Downloader Downloader(ImageStorage imageStorage, IUrlToStream urlToStream)
    {
        return new Downloader(imageStorage, urlToStream);
    }

    public virtual UrlGenerator UrlGenerator(CoordinatesGenerator coordinatesGenerator, ICoordinatesToUrl coordinatesToUrl)
    {
        return new UrlGenerator(coordinatesGenerator, coordinatesToUrl);
    }

    public virtual CoordinatesGenerator CoordinatesGenerator(ImageCoordinates maxImageCoordinates)
    {
        return new CoordinatesGenerator(maxImageCoordinates);

    }

    public virtual CoordinatesGenerator CoordinatesGenerator(ImageCoordinates maxImageCoordinates, int increaseStep)
    {
        return new CoordinatesGenerator(maxImageCoordinates, increaseStep);
    }

    public virtual ImageMontage ImageMontage(ImageStorage imageStorage)
    {
        return new ImageMontage(imageStorage);
    }
}

public class HandlerParametersCloserToVanEyck : HandlerParametersGeneric { }

