using libfragdown;
namespace LibfragdownTests;

internal class RemoteCoordinateStateTest : IRemoteCoordinateState
{
    private readonly int _horMax;
    private readonly int _verMax;

    public RemoteCoordinateStateTest(ImageCoordinates maxCoordinates)
    {
        _horMax = maxCoordinates.Horizontal;
        _verMax = maxCoordinates.Vertical;
    }

    public bool IsCoordinateExist(ImageCoordinates coordinates)
    {
        if ((coordinates.Vertical >= 0) &&
                (coordinates.Horizontal >= 0) &&
                (coordinates.Vertical <= _verMax) &&
                (coordinates.Horizontal <= _horMax))
            return true;
        return false;
    }
}
