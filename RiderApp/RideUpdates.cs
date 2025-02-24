namespace RiderApp;

internal sealed class RideUpdates
{
    public string Update { get; set; }
    public string Data { get; set; }
}

internal sealed class AcceptRide
{
    public DriverLocation DriverLocation { get; set; }
    public DriverInfo Driver { get; set; }
    public CabInfo Cab { get; set; }
    public RideInfo Ride { get; set; }
}

internal sealed class DriverInfo
{
    public string Name { get; set; }
    public string PhoneNo { get; set; }
    public string ProfileImageUrl { get; set; }
}

internal sealed class CabInfo
{
    public string LicensePlateNo { get; set; }
    public string Color { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string Manufacturer { get; set; }
}

internal sealed class RideInfo
{
    public long RideId { get; set; }
}
