using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace RiderApp;
public sealed class RideHubService : IDisposable
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
            })
            .AddMessagePackProtocol()
            .WithKeepAliveInterval(TimeSpan.FromSeconds(20))
            .Build();

        SubscribeToRideRequestUpdates();
        SubscribeToChatRequestUpdates();
        SubscribeToLocationUpdates();
        SubscribeToNearbyDrivers();
        SubscribeToRideUpdates();
    }

    public async Task StartAsync() => await _rideHub.StartAsync();

    public async Task Chat(string message)
    {
        if (_rideId == default) return;

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
            Console.ForegroundColor = ConsoleColor.Blue;
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
            Console.ForegroundColor = ConsoleColor.Blue;
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

            if (request.Update == ReceiveRideUpdate.Accepted)
            {
                var data = JsonSerializer.Deserialize<AcceptRide>(request.Data) ?? new();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Ride accepted. Details");
                Console.WriteLine($"Driver: {data.Driver.Name}");
                Console.WriteLine("Ride details: ");
                Console.WriteLine($"Cab: Manufacturer -> {data.Cab.Manufacturer}; Color -> {data.Cab.Color}");
                Console.WriteLine($"Location: Lat -> {data.DriverLocation.Latitude}; Long -> {data.DriverLocation.Longitude}");
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine($"This is ride id: {_rideId}");
            }

            if(request.Update == ReceiveRideUpdate.Started)
            {
                var data = JsonSerializer.Deserialize<DefaultUpdate>(request.Data) ?? new();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(data.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }

            if(request.Update == ReceiveRideUpdate.DriverArrived)
            {
                var data = JsonSerializer.Deserialize<DriverArrivedUpdate>(request.Data) ?? new();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(data.Message);
                Console.WriteLine(data.WaitingTimeInMinutes);
                Console.ForegroundColor = ConsoleColor.White;
            }

            if(request.Update == ReceiveRideUpdate.Ended)
            {
                var data = JsonSerializer.Deserialize<EndRideUpdate>(request.Data) ?? new();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Source: " + data.Source);
                Console.WriteLine("Destination: " + data.Destination);
                Console.WriteLine("Ride ended");
                Console.ForegroundColor = ConsoleColor.White;
            }

            if(request.Update == ReceiveRideUpdate.NoMatch)
            {
                var data = JsonSerializer.Deserialize<DefaultUpdate>(request.Data) ?? new();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(data.Message);
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
