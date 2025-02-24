namespace RiderApp;

internal abstract class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

internal sealed class RiderLocation : Location
{

}

internal sealed class DriverLocation : Location
{

}

internal sealed class NearbyDriver : Location
{
    internal string DriverKey { get; set;}
}
