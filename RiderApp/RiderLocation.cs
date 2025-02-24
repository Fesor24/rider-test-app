using System.Text.Json.Serialization;

namespace RiderApp;

public abstract class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public sealed class RiderLocation : Location
{

}

public sealed class DriverLocation : Location
{

}

public sealed class NearbyDriver : Location
{
    public string DriverKey { get; set;}
}
