using ChessAndQuests.DAL;
using ChessAndQuests.Hubs;
using ChessAndQuests.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.Tasks;

namespace ChessAndQuests.Controllers
{
    public class GameController : Controller
    {
        private readonly IHubContext<GameHub> _hubContext;

        public GameController(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }


        public IActionResult TestBoard()
        {
            return View();
        }

        [HttpGet]
        // create a new game get
        public IActionResult CreateGame()
        {
            if (HttpContext.Session.GetString("PlayerUsername") == null)
            {
                return RedirectToAction("SignIn", "Player");
            }
            return View();
        }

        [HttpPost]
        // create a new game post
        //en funktion som tar in indata och skapar ett nytt spel och returnerar ett GameDetails objekt
        //Skapa nytt spel i dal GameMethods.CreateGame(GameDetails)
        //returnera till spelbrädssidan med det nya spelet
        public IActionResult CreateGame(string gamekey, int playerId)
        {
            GameMethods gameMethods = new GameMethods();
            //hämta fensträng för startposition
            GameDetails newGame = new GameDetails();
            {
                newGame.PLayerWhiteId = playerId;
                newGame.GameKey = gamekey;
                newGame.CurrentFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1\r\n";
                newGame.status = 0;
                newGame.turnId = playerId;
            }
            int i = 0;
            string error = "";

            i = gameMethods.CreateGame(newGame, out error);

            if (i == 0)
            {
                ViewBag.ErrorGame = "Error creating game: " + error;
                return View();
            }
            PlayerMethods playerMethods = new PlayerMethods();
            var player = playerMethods.GetById(playerId, out error);

            return RedirectToAction("PlayGame","Game", new { gameKey = newGame.GameKey });
        }


        [HttpGet]
        // join a game get
        public IActionResult JoinGame()
        {
            if (HttpContext.Session.GetString("PlayerUsername") == null)
            {
                return RedirectToAction("SignIn", "Player");
            }


            return View();

        }


        [HttpPost]
        // join a game post
        public async Task<IActionResult> JoinGame(string gamekey, int playerId)
        {

            GameMethods gameMethods = new GameMethods();
            string error = "";
            int i = 0;
            GameDetails gameToJoin = gameMethods.GetGameByKey(gamekey, out error);

            if (gameToJoin == null)
            {
                ViewBag.ErrorJoin = "Game not found.";
                return View();
            }
            if (gameToJoin.PlayerBlackId != null)
            {
                ViewBag.ErrorJoin = "Game is already full.";
                return View();
            }
            PlayerMethods playerMethods = new PlayerMethods();
            var player = playerMethods.GetById(playerId, out error);
            gameToJoin.PlayerBlackId = playerId;
            i = gameMethods.UpdateGame(gameToJoin, out error);
            if(i==0)
            {
                ViewBag.ErrorJoin = "Error joining game: " + error;
                return View();
            }
            var whiteName = playerMethods.GetById(gameToJoin.PLayerWhiteId, out error)?.PlayerUserName ?? "PlayerWhite"; 
            var blackName = player?.PlayerUserName ?? "PlayerBlack";
            await _hubContext.Clients.Group(gameToJoin.GameKey).SendAsync("ReceivePlayerNames", whiteName, blackName, false);


            return RedirectToAction("PlayGame", "Game", new { gameKey = gameToJoin.GameKey });
        }
        [HttpGet]
        public IActionResult PlayGame(string gameKey)
        {
            if (HttpContext.Session.GetString("PlayerUsername") == null)
            {
                return RedirectToAction("SignIn", "Player");
            }
            GameMethods gameMethods = new GameMethods();
            GameDetails gameDetails = new GameDetails();
            PlayerDetails playerBlack = null;
            string error = "";

            gameDetails= gameMethods.GetGameByKey(gameKey, out  error);

            var model = new GameViewModel
            {
                GameKey = gameKey,
                CurrentFEN = gameDetails.CurrentFEN

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MakeMove([FromBody] MakeMoveDto dto)
        {
            //get game by key
            var gameMethods = new GameMethods();
            var game = gameMethods.GetGameByKey(dto.GameKey, out var error);
            if (game == null) {return BadRequest("Game not found: " + error);}

            //update game's current fen
            game.CurrentFEN = dto.Fen?.Trim() ?? game.CurrentFEN;
            gameMethods.UpdateGame(game, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest("Error updating game: " + error);
            }

            await _hubContext.Clients.Group(dto.GameKey).SendAsync("FenUpdated");

            return Ok();
        }

    }
}

public class MakeMoveDto
{
    public string GameKey { get; set; }
    public string FromSquare { get; set; }
    public string ToSquare { get; set; }
    public string Fen { get; set; }
}

