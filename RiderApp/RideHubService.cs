using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;

namespace RiderApp;

internal sealed class RideHubService : IDisposable
{
    private HubConnection _rideHub;

    private readonly string _connectionUrl;
    private readonly string _accessToken;
    private long _rideId;

    public RideHubService(string baseUrl, string accessToken)
    {
        _connectionUrl = baseUrl + "/ride-hub";
        _accessToken = accessToken;

        _rideHub = new HubConnectionBuilder()
            .WithUrl(_connectionUrl, options =>
            {
                options.Headers.Add("Authorization", "Bearer " + _accessToken);
            }).Build();

        SubscribeToRideRequestUpdates();
        SubscribeToChatRequestUpdates();
        SubscribeToLocationUpdates();
        SubscribeToNearbyDrivers();
        SubscribeToRideUpdates();
    }

    public async Task StartAsync() => await _rideHub.StartAsync();

    public async Task Chat(string message)
    {
        SendChatMessage rideChat = new()
        {
            RideId = _rideId,
            Message = message
        };

        await _rideHub.InvokeAsync("Chat", rideChat, CancellationToken.None);
    }

    public async Task GetNearbyDrivers()
    {
        RiderLocation riderLocation = new()
        {
            Longitude = 3.3898,
            Latitude = 6.5158
        };

        await _rideHub.InvokeAsync("GetNearbyDrivers", riderLocation, CancellationToken.None);
    }

    private void SubscribeToRideRequestUpdates()
    {
        _rideHub.On<RideRequest>("ReceiveRideRequests", (request) =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Request sent to driver. Driver Info:");
            Console.WriteLine($"First name: {request.Driver.FirstName}");
            Console.WriteLine($"Last name: {request.Driver.LastName}");
            Console.ForegroundColor = ConsoleColor.White;
        });
    }

    private void SubscribeToChatRequestUpdates()
    {
        _rideHub.On<RideChat>("ReceiveChatMessage", (request) =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Chats received");
            foreach (var chat in request.Chats)
            {
                Console.WriteLine($"{chat.Sender} to {chat.Recipient}: {chat.Message}");
            }

            Console.ForegroundColor = ConsoleColor.White;
        });
    }

    private void SubscribeToLocationUpdates()
    {
        _rideHub.On<DriverLocation>("ReceiveLocationUpdate", (request) =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Driver location update:");
            Console.WriteLine($"Latitude: {request.Latitude}");
            Console.WriteLine($"Longitude: {request.Longitude}");
            Console.ForegroundColor = ConsoleColor.White;
        });
    }

    private void SubscribeToNearbyDrivers()
    {
        _rideHub.On<NearbyDriver>("ReceiveNearbyDrivers", (request) =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Nearby driver location update:");
            Console.WriteLine($"Latitude: {request.Latitude}");
            Console.WriteLine($"Longitude: {request.Longitude}");
            Console.WriteLine($"Driver key: {request.DriverKey}");
            Console.ForegroundColor = ConsoleColor.White;
        });
    }

    private void SubscribeToRideUpdates()
    {
        _rideHub.On<RideUpdates>("ReceiveRideUpdates", (request) =>
        {
            Console.WriteLine($"Update event: {request.Update}");

            if (request.Update == "Accepted")
            {
                var data = JsonSerializer.Deserialize<AcceptRide>(request.Data, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (data is null) return;

                _rideId = data.Ride.RideId;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Ride accepted. Details");
                Console.WriteLine($"Driver: {data.Driver.Name}");
                Console.WriteLine("Ride details: ");
                Console.WriteLine($"Cab: Manufacturer -> {data.Cab.Manufacturer}; Color -> {data.Cab.Color}");
                Console.WriteLine($"Location: Lat -> {data.DriverLocation.Latitude}; Long -> {data.DriverLocation.Longitude}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (request.Update == "NoMatch")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No match for ride request");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (request.Update == "DriverArrived")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Your ride is here");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (request.Update == "Started")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Ride started");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if (request.Update == "Ended")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Ride ended");
                Console.ForegroundColor = ConsoleColor.White;
            }
        });
    }

    private void Close()
    {
        _rideHub.StopAsync();
    }

    public void Dispose()
    {
        Close();
    }
}
