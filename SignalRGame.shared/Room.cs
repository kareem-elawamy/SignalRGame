using SignalRGame.shared;

public class Room
{
    public string RoomId { get; set; }
    public string RoomName { get; set; }
    public List<Player> PlayerList { get; set; } = new();
    public Game game { get; set; } = new();

    public bool AddPlayer(Player player)
    {
        if (!PlayerList.Contains(player) && PlayerList.Count < 2)
        {
            PlayerList.Add(player);
            if (PlayerList.Count == 1)
                game.xplayerConId = player.connectionId;
            else
                game.oplayerConId = player.connectionId;

            return true;
        }
        return false;
    }
}
