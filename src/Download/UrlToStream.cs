namespace libfragdown;

public interface IUrlToStream
{
    public Stream GetStream(Uri url);
}

public class UrlToStreamGeneric : IUrlToStream
{
    private readonly HttpClient _httpClient = new();

    public Stream GetStream(Uri url)
    {
        Task<Stream> downloadTask = _httpClient.GetStreamAsync(url);
        return downloadTask.Result;
    }
}
