using ChessAndQuests.DAL;
using ChessAndQuests.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks;
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
        }


        public async Task SendPlayerNames(string gameKey)
        {
            var game = _gameMethods.GetGameByKey(gameKey, out _);
            var whiteId = game.PlayerWhiteId;
            var blackId = game.PlayerBlackId;
            var white = _playerMethods.GetById(whiteId, out _)?.PlayerUserName ?? "PlayerWhite";
            var black = game.PlayerBlackId.HasValue
                ? _playerMethods.GetById(blackId.Value, out _)?.PlayerUserName ?? "PlayerBlack"
                : "Waiting...";
            bool isWaiting = !game.PlayerBlackId.HasValue;

            await Clients.Group(gameKey).SendAsync("ReceivePlayerNames", white, black, isWaiting, whiteId, blackId);
        }

        // Broadcast the latest FEN to all clients in the game group
        public async Task BroadcastLatestFen(string gameKey)
        {
            var game = _gameMethods.GetGameByKey(gameKey, out _);
            if (gameKey == null) return;
            GameViewModel gamevm = new GameViewModel
            {
     
                CurrentFEN = game.CurrentFEN,
                TurnPlayerId = game.turnId
            };
            await Clients.Group(gameKey).SendAsync("ReceiveLatestFen", gamevm); //2


        }

        //Notify clients about checkmate

        public async Task NotifyCheckmate(string gameKey, int winnerPlayerId)
        {
            await Clients.Group(gameKey).SendAsync("GameIsFinished", winnerPlayerId);
            DeleteGame(gameKey);

        }
        //Notify clients about stalemate
        public async Task NotifyStalemate(string gameKey)
        {
            await Clients.Group(gameKey).SendAsync("GameIsFinished", "stalemate");
            DeleteGame(gameKey);

        }
        private void DeleteGame (string gameKey)
        {
            GameMethods gameMethods = new GameMethods();
            string error = "";
            int i = 0;
            GameDetails gameToDelete = gameMethods.GetGameByKey(gameKey, out error);
            if (gameToDelete != null)
            {
                i = gameMethods.DeleteGame(gameToDelete.GameId, out error);
            }
            
            
        }
    }
}
