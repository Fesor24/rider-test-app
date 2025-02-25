using RiderApp;

Console.WriteLine("Riders app!!!");

string baseUrl = "https://stagingapi.soloride.app";

Console.WriteLine("Paste riders access token below: ");

string? accessToken = Console.ReadLine();

while (string.IsNullOrWhiteSpace(accessToken))
    accessToken = Console.ReadLine();

Console.Clear();

Console.WriteLine("To get nearby drivers. Input location information below:");
Console.WriteLine("Your Latitude: ");
if (!double.TryParse(Console.ReadLine(), out double lat)) lat = 0;

Console.WriteLine("Your Longitude: ");
if(!double.TryParse(Console.ReadLine(), out double longitude)) longitude = 0;

using var rideHubService = new RideHubService(baseUrl, accessToken);

Task nearbyDriverTask = GetNearbyDrivers();

Task chatTask = Chat();

await Task.WhenAll(nearbyDriverTask, chatTask);

async Task Chat()
{
    Console.WriteLine("Enter a message to chat if there's a ride match: ");
    string message = await Console.In.ReadLineAsync() ?? "";

    while (!string.IsNullOrWhiteSpace(message))
    {
        await rideHubService.Chat(message);

        Console.WriteLine("Enter a message: ");
        message = await Console.In.ReadLineAsync() ?? "";
    }
}

async Task GetNearbyDrivers()
{
    while (true)
    {
        await rideHubService.GetNearbyDrivers(lat, longitude);

        await Task.Delay(60000 * 3); // 3 mins...
    }
}
