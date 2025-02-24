namespace RiderApp;

public sealed class RideRequest
{
    public RideRequestDriverInfo Driver { get; set; }
}

public sealed class RideRequestDriverInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfileImageUrl { get; set; }
}
