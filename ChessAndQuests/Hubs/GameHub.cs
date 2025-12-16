using ChessAndQuests.DAL;
using Microsoft.AspNetCore.SignalR;
namespace ChessAndQuests.Hubs
{
    public class GameHub : Hub
    {
        private readonly GameMethods _gameMethods;
        private readonly PlayerMethods _playerMethods;
        public GameHub()
        {
            _gameMethods = new GameMethods();
            _playerMethods = new PlayerMethods();
        }
        public async Task JoinGameGroup(string gameKey)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameKey);
            await SendPlayerNames(gameKey);
            await BroadcastLatestFen(gameKey);
        }


        public async Task SendPlayerNames(string gameKey)
        {
            var game = _gameMethods.GetGameByKey(gameKey, out _);
            var white = _playerMethods.GetById(game.PLayerWhiteId, out _)?.PlayerUserName ?? "PlayerWhite";
            var black = game.PlayerBlackId.HasValue
                ? _playerMethods.GetById(game.PlayerBlackId.Value, out _)?.PlayerUserName ?? "PlayerBlack"
                : "Waiting...";
            bool isWaiting = !game.PlayerBlackId.HasValue;

            await Clients.Group(gameKey).SendAsync("ReceivePlayerNames", white, black, isWaiting);
        }

        // Broadcast the latest FEN to all clients in the game group
        public async Task BroadcastLatestFen( string gameKey)
        {
            var game = _gameMethods.GetGameByKey(gameKey, out _);
            if (gameKey == null) return;
            await Clients.Group(gameKey).SendAsync("ReceiveLatestFen", game.CurrentFEN);


        }
    }
}
