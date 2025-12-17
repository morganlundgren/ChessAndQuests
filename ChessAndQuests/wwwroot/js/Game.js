
// ------------------- Game.js -----------------------

var connection = new signalR.HubConnectionBuilder().withUrl("/gamehub").build();
const gameKey = document.getElementById("gamekey").dataset.gameKey;
var game = new Chess(start_fen);
let board = null;
let whitePlayerId = null;
let blackPlayerId = null;
let winnerImage = "url('/images/winner.png')";
let loserImage = "url('/images/loser.png')";





// ---------------- FUNCTIONS FOR CHESSBOARD.JS ----------------
function onDragStart(source, piece) {
    // do not pick up pieces if the game is over
    if (game.isGameOver()) return false;

    

    if ((game.turn() === 'w' && currentPlayerId !== whitePlayerId) ||
        (game.turn() === 'b' && currentPlayerId !== blackPlayerId)) {
        return false; 
    }

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
    if (move === null) {
        return 'snapback';
    }


    updateActivePlayer();
    sendMoveToServer(source, target, game.fen());

    if (game.isCheckmate()) {
        connection.invoke("NotifyCheckmate", gameKey, currentPlayerId);
        deleteGameOnMate();
    }
}



function sendMoveToServer(from, to, fen) {
    fetch('/Game/MakeMove', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            GameKey: gameKey,
            FromSquare: from,
            ToSquare: to,
            CurrentFEN: fen,
            TurnPlayerId: currentPlayerId
        })
    });
}

function updateActivePlayer() {
    const whiteCard = document.querySelector('.player-card.white');
    const blackCard = document.querySelector('.player-card.black');

    if (!whiteCard || !blackCard) return;

    whiteCard.classList.remove('active');
    blackCard.classList.remove('active');

    if (game.turn() === 'w') {
        whiteCard.classList.add('active');
    } else {
        blackCard.classList.add('active');
    }
}

function deleteGameOnMate() {
    if (game.isCheckmate()) {
        fetch('/Game/DeleteGame', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                GameKey: gameKey
            })
        });
    }
}


// ---------------- SIGNALR CONNECTION ----------------

connection.start().then(() => {
    console.log("Connected to SignalR");
    connection.invoke("JoinGameGroup", gameKey);
    connection.invoke("BrodcastLatestFen", gameKey);

});


connection.on("ReceivePlayerNames", (whiteName, blackName, isWaiting, whiteId, blackId) => {

    document.getElementById("waitingOverlay").style.display = isWaiting ? "flex" : "none";

    document.getElementById("playerWhite").textContent = whiteName;
    document.getElementById("playerBlack").textContent = blackName;
    document.getElementById("playerWhite").dataset.whiteId = whiteId;
    document.getElementById("playerBlack").dataset.blackId = blackId;
    whitePlayerId = whiteId;
    blackPlayerId = blackId;

    if (!board) {

        const orientation = (currentPlayerId === whiteId) ? 'white' : 'black';

            board = ChessBoard('board', {
            draggable: true,
            position: start_fen,
            pieceTheme: '/images/chesspieces/alpha/{piece}.png',
            onDragStart: onDragStart,
            onDrop: onDrop,
            orientation: orientation
            
            });
            setTimeout(() => {
                board.resize();
            }, 0);
            window.addEventListener('resize', () => {
                if (board) board.resize();
            });


        updateActivePlayer();
    }
});

connection.on("ReceiveLatestFen", (fen) => {
    console.log("ReceviveFen:", fen);

    if (game === null) {
        game = new Chess(start_fen);
    }
    game.load(fen);
    board.position(fen);
    updateActivePlayer();
});

connection.on("GameIsFinished", (winner) => {
    document.getElementById("gameOverOverlay").style.display = "flex";
    if (winner === currentPlayerId) {
        document.getElementById("gameOverMessage").style.backgroundImage = winnerImage;
    } else {
        document.getElementById("gameOverMessage").style.backgroundImage = loserImage;
    }

});







