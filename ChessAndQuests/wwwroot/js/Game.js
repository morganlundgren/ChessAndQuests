
// ------------------- Game.js -----------------------

var connection = new signalR.HubConnectionBuilder().withUrl("/gamehub").build();
const gameKey = document.getElementById("gamekey").dataset.gameKey;
var game = new Chess(start_fen);
let board = null;
let whitePlayerId = null;
let blackPlayerId = null;
let currentTurnPlayerId = null;
let pendingPromotion = null;

let winnerImage = "url('../Images/winner.png')";
let loserImage = "url('../Images/loser.png')";
let stalemateImage = "url('../Images/stalemate.png')";







//-------------------------------- PROMOTION HANDLING (not working) ------------------------------   
function isPromotionMove(source, target) {
    const piece = game.get(source)
    if (!piece) return false;
    if (piece.type !== 'p') return false;

    return (
        (piece.color === 'w' && target[1] === '8') ||
        (piece.color === 'b' && target[1] === '1')
    );
}

function showPromotionDialog(from, to) {
    pendingPromotion = { from, to };
    const overlay = document.getElementById('promotionOverlay')

    const square = document.querySelector(`.square-${to}`);
    if (square) {

        const rect = square.getBoundingClientRect();
        overlay.style.position = 'absolute';
        overlay.style.top = `${rect.top + window.scrollY}px`;   
        overlay.style.left = `${rect.left + window.scrollX}px`;
    }
    overlay.style.display = 'flex';
}

document.querySelectorAll('#promotionOverlay button')
    .forEach(btn => {
        btn.addEventListener('click', () => {
            const promotion = btn.dataset.piece;
            completePromotion(promotion);
        });
    });
function completePromotion(promotion) {

    const { from, to } = pendingPromotion;
    document.getElementById('promotionOverlay').style.display = 'none';
    pendingPromotion = null;

    var move;
    try {
        move = game.move({
            from,
            to,
            promotion
        });
    } catch (e) {
        // ogiltigt drag → snapback
        return 'snapback';
    }

    sendMoveToServer(from, to, game.fen());
    checkGameEnd();
}

// ---------------- FUNCTIONS FOR CHESSBOARD.JS ----------------
function onDragStart(source, piece) {
    // do not pick up pieces if the game is over
    if (game.isGameOver()) return false;


    // kontrollera tur baserat på DB

    if (currentPlayerId !== currentTurnPlayerId) {
        return false;
    }


    if ((game.turn() === 'w' && piece.startsWith('b')) ||
        (game.turn() === 'b' && piece.startsWith('w'))) {
        return false;
    }


}
function onDrop(source, target) { //4

    if (source === target) {

        return 'snapback';
    }

    // Promotion handling (not working)

    if (isPromotionMove(source, target)) {
        showPromotionDialog(source, target);
        return;
    }

    var move;
    try {
        move = game.move({
            from: source,
            to: target
        });
    } catch (e) {
        // ogiltigt drag → snapback
        return 'snapback';
    }
    sendMoveToServer(source, target, game.fen());
    checkGameEnd();
}

function checkGameEnd() {

    if (game.isCheckmate()) {
        connection.invoke("NotifyCheckmate", gameKey, currentPlayerId);
        deleteGameOnMate();
    } else if (game.isStalemate()) {
        connection.invoke("NotifyStalemate", gameKey);
        deleteGameOnMate();
    }
}

function updateActivePlayer() {
    const whiteCard = document.getElementById('whiteCard');
    const blackCard = document.getElementById('blackCard');

    if (!whiteCard || !blackCard) return;

    whiteCard.classList.remove('active');
    blackCard.classList.remove('active');

    if (currentTurnPlayerId === whitePlayerId) {
        whiteCard.classList.add('active');
    } else if (currentTurnPlayerId === blackPlayerId) {
        blackCard.classList.add('active');
    }
}



function sendMoveToServer(from, to, fen) {//5
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



function deleteGameOnMate() {

    fetch('/Game/DeleteGame', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            GameKey: gameKey
        })
    });
}


// ---------------- SIGNALR CONNECTION ----------------

connection.start().then(() => {
    console.log("Connected to SignalR");
    connection.invoke("JoinGameGroup", gameKey);
    connection.invoke("BrodcastLatestFen", gameKey); //1

});


connection.on("ReceivePlayerNames", (whiteName, blackName, isWaiting, whiteId, blackId) => {

    document.getElementById("waitingOverlay").style.display = isWaiting ? "flex" : "none";

    document.getElementById("playerWhite").textContent = whiteName;
    document.getElementById("playerBlack").textContent = blackName;
    document.getElementById("playerWhite").dataset.whiteId = whiteId;
    document.getElementById("playerBlack").dataset.blackId = blackId;
    whitePlayerId = whiteId;
    blackPlayerId = blackId;
    currentTurnPlayerId = whiteId;

    if (!board) {

        const orientation = (currentPlayerId === whiteId) ? 'white' : 'black';
        const whiteCard = document.getElementById('whiteCard');
        const blackCard = document.getElementById('blackCard');

        const top = document.getElementById('playerTop');
        const bottom = document.getElementById('playerBottom');
        top.innerHTML = '';
        bottom.innerHTML = '';

        if (orientation === 'white') {
            top.appendChild(blackCard);
            bottom.appendChild(whiteCard);
        } else {
            top.appendChild(whiteCard);
            bottom.appendChild(blackCard);
        }

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

connection.on("ReceiveLatestFen", (fen, turnPlayerId) => { //3
    console.log("ReceviveFen:", fen);

    if (game === null) {
        game = new Chess(start_fen);
    }
    game.load(fen);
    board.position(fen);
    currentTurnPlayerId = turnPlayerId;
    updateActivePlayer();
});

connection.on("GameIsFinished", (result) => {
    document.getElementById("gameOverOverlay").style.display = "flex";

    if (result === "stalemate") {
        document.getElementById("gameOverMessage").style.backgroundImage = stalemateImage;
    } else if (result === currentPlayerId) {
        document.getElementById("gameOverMessage").style.backgroundImage = winnerImage;
    } else {
        document.getElementById("gameOverMessage").style.backgroundImage = loserImage;
    }

});

// ---------------- FORFEIT ----------------
document.getElementById("forfeitButton").addEventListener("click", () => {
    if (confirm("Are you sure you want to forfeit the game?")) {
        const winnerId = (currentPlayerId === whitePlayerId) ? blackPlayerId : whitePlayerId;
        connection.invoke("NotifyCheckmate", gameKey, winnerId)
            .catch(err => console.error(err.toString()));
        deleteGameOnMate();
    }
});







