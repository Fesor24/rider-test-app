namespace RiderApp;

public sealed class RideUpdates
{
    public string Update { get; set; }
    public string Data { get; set; }
}

public class DefaultUpdate
{
    public string Message { get; set; }
}

public sealed class DriverArrivedUpdate : DefaultUpdate
{
    public int WaitingTimeInMinutes { get; set; }
}

public sealed class EndRideUpdate
{
    public string Source { get; set; }
    public string Destination { get; set; }
    public long TotalFareAmount { get; set; }
    public long FareOutstanding { get; set; }
}

public sealed class AcceptRide
{
    public DriverLocation DriverLocation { get; set; }
    public DriverInfo Driver { get; set; }
    public CabInfo Cab { get; set; }
    public RideInfo Ride { get; set; }
}

public sealed class DriverInfo
{
    public string Name { get; set; }
    public string PhoneNo { get; set; }
    public string ProfileImageUrl { get; set; }
}

public sealed class CabInfo
{
    public string LicensePlateNo { get; set; }
    public string Color { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string Manufacturer { get; set; }
}

public sealed class RideInfo
{
    public long RideId { get; set; }
}

public static class ReceiveRideUpdate
{
    public static string Accepted = nameof(Accepted);
    public static string Cancelled = nameof(Cancelled);
    public static string NoMatch = nameof(NoMatch);
    public static string DriverArrived = nameof(DriverArrived);
    public static string Started = nameof(Started);
    public static string Ended = nameof(Ended);
    public static string Reassigned = nameof(Reassigned);
    public static string Rerouted = nameof(Rerouted);
}
