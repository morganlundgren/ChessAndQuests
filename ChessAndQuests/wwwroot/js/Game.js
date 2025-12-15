var connection = new signalR.HubConnectionBuilder().withUrl("/gamehub").build();
const gameKey = document.getElementById("gamekey").dataset.gameKey;


connection.start().then(() => {
    console.log("Connected to SignalR");
    connection.invoke("JoinGameGroup", gameKey);
    setInterval(() => {
        connection.invoke("PollGameState", gameKey);
    }, 2000);
});


connection.on("ReceivePlayerNames", (whiteName, blackName, isWaiting) => {

    document.getElementById("waitingOverlay").style.display = isWaiting ? "flex" : "none";

    document.getElementById("playerWhite").textContent = whiteName;
    document.getElementById("playerBlack").textContent = blackName;
   
});

var game = new Chess(start_fen);
var board = null;

function onDragStart(source, piece) {
        // do not pick up pieces if the game is over
    if (game.isGameOver()) return false;

    if ((game.turn() === 'w' && piece.startsWith('b')) ||
        (game.turn() === 'b' && piece.startsWith('w'))) {
        return false;

    }
}

function onDrop(source, target) {
    var move = game.move({
        from: source,
        to: target,
        promotion: 'q'// NOTE: always promote to a queen for example simplicity
    });

    // illegal move
    if (move === null) return 'snapback';

    sendMoveToServer(source, target, game.fen());
}

function sendMoveToServer(from, to, fen) {
    fetch('/Game/MakeMove', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            gameKey: gameKey,
            fromSquare: from,
            toSquare: to,
            fen: fen
        })
    });
}

board = ChessBoard('board', {
    draggable: true,
    position: start_fen,
    pieceTheme: '/images/chesspieces/alpha/{piece}.png',
    onDragStart: onDragStart,
    onDrop: onDrop
});

