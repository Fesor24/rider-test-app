namespace RiderApp;

internal sealed class RideRequest
{
    public RideRequestDriverInfo Driver { get; set; }
}

internal sealed class RideRequestDriverInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfileImageUrl { get; set; }
}
