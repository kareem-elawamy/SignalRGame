using Microsoft.AspNetCore.SignalR;
using SignalRGame.shared;
using System.Text.RegularExpressions;

public class GameHub : Hub
{
    private static readonly List<Room> rooms = new();

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Connected: {Context.ConnectionId}");
        await Clients.Caller.SendAsync("Rooms", rooms.OrderBy(r => r.RoomName).ToList());
    }

    public async Task<Room> CreateRoom(string playerName, string roomName)
    {
        var player = new Player
        {
            connectionId = Context.ConnectionId,
            name = playerName,
            playerId = Guid.NewGuid().ToString()
        };

        var room = new Room
        {
            RoomId = Guid.NewGuid().ToString(),
            RoomName = roomName
        };

        if (room.AddPlayer(player))
        {
            rooms.Add(room);
            await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomId);
            await Clients.All.SendAsync("Rooms", rooms.OrderBy(r => r.RoomName).ToList());
            return room;
        }

        return null;
    }

    public async Task<Room> JoinRoom(string roomId, string playerName)
    {
        var room = rooms.FirstOrDefault(r => r.RoomId == roomId);
        if (room == null || room.PlayerList.Count >= 2)
            return null;

        var player = new Player
        {
            connectionId = Context.ConnectionId,
            name = playerName,
            playerId = Guid.NewGuid().ToString()
        };

        if (room.AddPlayer(player))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomId);
            await Clients.All.SendAsync("Rooms", rooms.OrderBy(r => r.RoomName).ToList());
            return room;
        }

        return null;
    }

    public async Task MakeMove(string roomId, int index, string symbol)
    {
        await Clients.Group(roomId).SendAsync("ReceiveMove", index, symbol);
    }
}
