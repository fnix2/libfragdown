namespace libfragdown
{
    public interface IRemoteCoordinateState
    {
        public bool IsCoordinateExist(ImageCoordinates coordinates);
    }

    public class RemoteCoordinateStateGeneric : IRemoteCoordinateState
    {
        private readonly ICoordinatesToUrl _convert;
        private static readonly HttpClient _httpClient = new();

        public RemoteCoordinateStateGeneric(ICoordinatesToUrl convertCoordinatesToUrl)
        {
            _convert = convertCoordinatesToUrl;
        }

        public bool IsCoordinateExist(ImageCoordinates coordinates)
        {
            return IsFileExist(_convert.Convert(coordinates));
        }

        private bool IsFileExist(Uri url)
        {
            using HttpRequestMessage request = new(HttpMethod.Head, url); //TODO: Может застревать сдесь при проблемах с интернетом
            using HttpResponseMessage result = _httpClient.Send(request);
            return (int)result.StatusCode == 200;
        }
    }
}
