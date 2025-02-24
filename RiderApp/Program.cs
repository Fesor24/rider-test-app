using RiderApp;

Console.WriteLine("Riders app!!!");

string baseUrl = "https://localhost:7175";

// paste riders access token here...
string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJodHRwczovL2Rldi5zb2xvcmlkZS5hcHAiLCJpc3MiOiJodHRwczovL2Rldi5zb2xvcmlkZS5hcHAiLCJleHAiOjUzNDAzODcwODEsImlhdCI6MTc0MDM4NzA4MSwibmJmIjoxNzQwMzg3MDgxLCJlbWFpbCI6ImZlc29yQG1haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiJSSURFUklELTEiLCJyaWRlciI6IjEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJSaWRlciJ9.wITrTg56xWhtuTcWWI2i5Vb979j4cOb8R0FWqWpqMOg";

using var rideHubService = new RideHubService(baseUrl, accessToken);

await rideHubService.StartAsync();

Task nearbyDriverTask = rideHubService.GetNearbyDrivers();

Task chatTask = Task.Run(async () => await Chat());

await Task.WhenAll(nearbyDriverTask, chatTask);

async Task Chat()
{
    Console.WriteLine("Enter a message to chat if there's a ride match: ");
    string message = await Task.Run(() => Console.ReadLine() ?? "");

    while (!string.IsNullOrWhiteSpace(message))
    {
        await rideHubService.Chat(message);

        Console.WriteLine("Enter a message: ");
        message = await Task.Run(() => Console.ReadLine() ?? "");
    }
}
