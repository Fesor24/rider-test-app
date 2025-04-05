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

public sealed class CallNotification
{
    public string CreatedAt { get; set; }
    public int ExpiryInSeconds { get; set; }
    public long RideId { get; set; }
    public CallerInfo CallerInfo { get; set; }
    public CallInfo CallInfo { get; set; }

}

public class CallerInfo
{
    public string Caller { get; set; }
    public string Name { get; set;}
    public string ProfileImage { get; set; }
}

public class CallInfo
{
    public string Recipient { get; set; }
    public string Token { get; set; }
    public string Channel { get; set; }
}

public class EmailVerificationMessage
{
    public string Message { get; set; }
    public bool Verified { get; set; }
}

