
// ------------------- Game.js -----------------------

var connection = new signalR.HubConnectionBuilder().withUrl("/gamehub").build();
const gameKey = document.getElementById("gamekey").dataset.gameKey;
var game = new Chess(start_fen);
let board = null;
let whitePlayerId = null;
let blackPlayerId = null;
let currentTurnPlayerId = null;
let pendingPromotion = null;
let undoAvailable = false;
let extraMoveAvalible = false;
let threatMovesLeft = 0;

let winnerImage = "url('../Images/winner.png')";
let loserImage = "url('../Images/loser.png')";
let stalemateImage = "url('../Images/stalemate.png')";







//-------------------------------- PROMOTION HANDLING  ------------------------------   
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

    sendMoveToServer(from, to, game.fen(), move.piece, move.captured);
    checkGameEnd();
}
//-------------------------------- Highlight squares------------------------------
function clearLegalMovesHighlights() {
    document.querySelectorAll('.square-legal-move')
        .forEach(el => el.classList.remove('square-legal-move')); //clear all highlights
}

function highlightLegalMoves(source) {

    clearLegalMovesHighlights(); // clear highlight of previously selected piece

    const moves = game.moves({ // chess.js function for getting legal moves. 
        square: source,
        verbose: true    //verbose:true returns objects with from/to (move obejcts)
    });
    moves.forEach(m => {
        const square = document.querySelector(`.square-${m.to}`);
        if (square) {
            square.classList.add('square-legal-move');// use he move objects "to" to highlight squares (legal moves))
        }
    });
}

function clearLastMoveHighlight() {
    document.querySelectorAll('.square-last-move')
        .forEach(el => el.classList.remove('square-last-move')); //clear all highlights
}

function highlightLastMove(from, to) {
    clearLastMoveHighlight();

    const fromSquare = document.querySelector(`.square-${from}`);
    const toSquare = document.querySelector(`.square-${to}`);

    if (fromSquare) fromSquare.classList.add('square-last-move');
    if (toSquare) toSquare.classList.add('square-last-move');
}

// ---------------- FUNCTIONS FOR CHESSBOARD.JS ----------------
function onDragStart(source, piece) {
    // do not pick up pieces if the game is over
    if (game.isGameOver()) return false;


    // kontrollera tur baserat på DB

    if (currentPlayerId !== currentTurnPlayerId) {
        return false;
    }

    if ((currentTurnPlayerId === whitePlayerId && piece.startsWith('b')) ||
        (currentTurnPlayerId === blackPlayerId && piece.startsWith('w'))) {
        return false;
    }
    highlightLegalMoves(source); // highlight legal moves from the selected square


}
function onDrop(source, target) { //4

    clearLegalMovesHighlights(); // clear highlights when a piece is dropped
    if (source === target) {

        return 'snapback';
    }

    // check if treatmoves are active
   /* if (threatHighlightMovesLeft > 0) {
        const threatened = getThreatenedPieces(game);
        threatened.forEach(square => highlightSquare(square));
        threatMovesLeft--;
    }*/

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
    sendMoveToServer(source, target, game.fen(), move.piece, move.captured);
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



function sendMoveToServer(from, to, fen, piece, captured) {//5
    fetch('/Game/MakeMove', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            GameKey: gameKey,
            FromSquare: from,
            ToSquare: to,
            CurrentFEN: fen,
            TurnPlayerId: currentPlayerId,
            MovedPiece: piece,
            CapturedPiece: captured
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

// ---------------- QUEST LOGIC FUNCTIONS ----------------
function getQuestPerspective(state) {
    let myQuest, opponentQuest;

    if (currentPlayerId === whitePlayerId) {
        myQuest = state.whitePlayerQuest;
        opponentQuest = state.blackPlayerQuest;
    } else {
        myQuest = state.blackPlayerQuest;
        opponentQuest = state.whitePlayerQuest;
    }

    return { myQuest, opponentQuest };
}

function updateQuestProgress(currentQuest, myQuest, opponentQuest)
{
    document.getElementById("questTitle").textContent = currentQuest.questName;
    document.getElementById("questDescription").textContent = currentQuest.questDescription;

    document.getElementById("myPlayerQuest").textContent = `Moves left:\n${currentQuest.questMaxMoves - myQuest.playerQuestCurrentMove}`;

    if (currentQuest.questMaxProgressMoves) { /*questet använder progressmoves MÅSTE FIXAS */
        document.getElementById("myQuestProgress").style.display = "block";
        document.getElementById("myQuestProgress").textContent = `My progress:\n${myQuest.progressMoves} / ${currentQuest.questMaxProgressMoves}`;/*kravet för progressMoves MÅSTE FIXAS*/
        document.getElementById("opponentQuestProgress").style.display = "block";
        document.getElementById("opponentQuestProgress").textContent = `Oppenent progress:\n${opponentQuest.progressMoves} / ${currentQuest.questMaxProgressMoves}`;/*kravet för progressMoves MÅSTE FIXAS*/
    } else {
        document.getElementById("myQuestProgress").style.display = "none";
        document.getElementById("opponentQuestProgress").style.display = "none";
    }
}
function handleQuestReward(state) {
    switch (state.completedQuest.questRewards) {
        case "UNDO":
            enableUndoMove();
            break;
        case "EXTRA_TURN": 
            
            if (currentPlayerId === state.questWinnerId) {
                document.getElementById("questConfirmation").style.display = "flex";
                document.getElementById("questConfirmationText").style.color = "green";
                document.getElementById("questConfirmationText").textContent = "You have earned an extra turn! Please make your next move.";
            }
            else if (currentPlayerId !== state.questWinnerId && state.questWinnerId) {
                document.getElementById("questConfirmation").style.display = "flex";
                document.getElementById("questConfirmationText").style.color = "red";
                document.getElementById("questConfirmationText").textContent = "You´re opponent earned an extra turn! Watch out!";
            } else if (!state.questWinnerId){
                document.getElementById("questConfirmation").style.display = "flex";
                document.getElementById("questConfirmationText").textContent = "No one finished the quest, prepare for the next one.";
            }
                
            break;
        case "HIGHLIGHT_TREATS":
            //highlightTreatPieces();
            break;
        default:
            console.log("Unknown quest reward:", questReward);
    }
}

// get threatened squares

function getThreatenedSquares(game) {
    const threatenedSquares = new Set();
    const opponentColor = (game.turn() === 'w') ? 'b' : 'w'; //nej mpste hanteras via currentTurnPlayerId

    game.SQUARES.forEach(square => {
        const piece = game.get(square);

        if (piece && piece.color === opponentColor) {
            const moves = game.moves({ square: square, verbose: true });

            moves.forEach(move => {
                threatenedSquares.add(move.to);
            });
        }
    });

    return Array.from(threatenedSquares);
}

// highlight threatened pieces
function getThreatenedPieces(game) {
    threatMovesLeft = 5; // reset counter
    const threatenedSquares = getThreatenedSquares(game);

    return threatenedSquares.filter(square => {
        const piece = game.get(square);
        return piece && piece.color === game.turn();
    });
}


// button enabling for undo move
function enableUndoMove() {
    undoAvailable = true;
    document.getElementById("undoButton").disabled = false;
}



// wait for click 
/*document.getElementById("undoButton").addEventListener("click", () => {
    if (!undoAvailable) return;

    connection.invoke("RequestUndo", gameKey);// måste hämta fen-strängen innan ens drag gjordes. 
    undoAvailable = false                     // dessutom kolla ifall det är ens tur eller inte
    document.getElementById("undoButton").disabled = true; 
});                                                  
  */                                                       




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

connection.on("ReceiveLatestFen", (state) => { //3
    console.log("ReceviveFen:", state.currentFEN);

    clearLegalMovesHighlights(); // only used for safety reasons

    if (game === null) {
        game = new Chess(start_fen);
    }
    game.load(state.currentFEN);
    board.position(state.currentFEN);
    currentTurnPlayerId = state.turnPlayerId;

     const { myQuest, opponentQuest } = getQuestPerspective(state);

    if (state.questCompleted) {
        handleQuestReward(state);
        updateQuestProgress(state.currentQuest, myQuest, opponentQuest);

    }
    else if (state.currentQuest) {
        updateQuestProgress(state.currentQuest, myQuest, opponentQuest);
        document.getElementById("questConfirmation").style.display = "none";
    }
    

    if (state.fromSquare&& state.toSquare) {
        highlightLastMove(state.fromSquare, state.toSquare);
        document.getElementById("lastMoveText").textContent = 
            `Latest move:\n${state.fromSquare} → ${state.toSquare}`;
    }
    

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









