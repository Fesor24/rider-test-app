using System.Text.Json;
using RiderApp;

Console.WriteLine("Riders app!!!");

string baseUrl = "https://stagingapi.soloride.app";

string filePath = "token.txt";

string fileContent;

string accessToken;

try
{
    using var streamReader = new StreamReader(filePath);

    fileContent = streamReader.ReadToEnd();
}
catch (FileNotFoundException)
{
    fileContent = string.Empty;
}

Console.WriteLine("Enter 'y' to use cached data or 'n' to paste new token? [y/n]");

string response = Console.ReadLine();

if (response == "y")
{
    accessToken = await GetOrUpdateTokenIfExpired(fileContent, filePath);
}
else
{
    accessToken = GetToken();
    await SaveToken(accessToken, filePath);
}

Console.Clear();

Console.WriteLine("To get nearby drivers. Input location information below:");
Console.WriteLine("Your Latitude: ");
if (!double.TryParse(Console.ReadLine(), out double lat)) lat = 0;

Console.WriteLine("Your Longitude: ");
if(!double.TryParse(Console.ReadLine(), out double longitude)) longitude = 0;

using var rideHubService = new RideHubService(baseUrl, accessToken);

await rideHubService.StartAsync();

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

string GetToken()
{
    Console.WriteLine("Paste riders access token below: ");
    string? accessToken = Console.ReadLine();

    while (string.IsNullOrWhiteSpace(accessToken))
        accessToken = Console.ReadLine();

    return accessToken;
}

async Task<string> GetOrUpdateTokenIfExpired(string content, string path)
{
    string? accessToken;

    if (string.IsNullOrWhiteSpace(content))
    {
        Console.WriteLine("No saved data");
        accessToken = GetToken();

        content = JsonSerializer.Serialize(new CachedData(accessToken, DateTime.UtcNow.AddHours(4)));

        using var streamWriter = new StreamWriter(path);

        await streamWriter.WriteLineAsync(content);
    }
    else
    {
        var cachedData = JsonSerializer.Deserialize<CachedData>(content);

        if (cachedData is null || cachedData.Expiry <= DateTime.UtcNow)
        {
            Console.WriteLine("Token expired");
            accessToken = GetToken();

            content = JsonSerializer.Serialize(new CachedData(accessToken, DateTime.UtcNow.AddHours(4)));

            using var streamWriter = new StreamWriter(path);

            await streamWriter.WriteLineAsync(content);
        }
        else
        {
            accessToken = cachedData.AccessToken;
        }
    }

    return accessToken;
}

async Task SaveToken(string accessToken, string path)
{
    string content = JsonSerializer.Serialize(new CachedData(accessToken, DateTime.UtcNow.AddHours(4)));

    using var streamWriter = new StreamWriter(path);

    await streamWriter.WriteLineAsync(content);
}

sealed record CachedData(string AccessToken, DateTime Expiry);
