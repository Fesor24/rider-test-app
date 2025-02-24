using RiderApp;

Console.WriteLine("Riders app!!!");

string baseUrl = "https://stagingapi.soloride.app";

// paste riders access token here...
string accessToken = "";

using var rideHubService = new RideHubService(baseUrl, accessToken);

Task nearbyDriverTask = rideHubService.GetNearbyDrivers();

Task chatTask = Task.Run(async () => await Chat());

await Task.WhenAll(nearbyDriverTask, chatTask);

async Task Chat()
{
    Console.WriteLine("Enter a message to chat if there's a ride match: ");
    string message = Console.ReadLine() ?? "";

    while (string.IsNullOrWhiteSpace(message))
    {
        await rideHubService.Chat(message);

        Console.WriteLine("Enter a message: ");
        message = Console.ReadLine() ?? "";
    }
}
