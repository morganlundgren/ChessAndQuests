var connection = new signalR.HubConnectionBuilder().withUrl("/gamehub").build();
const gameKey = document.getElementById("gamekey").dataset.gameKey;


connection.start().then(() => {
    console.log("Connected to SignalR");
    connection.invoke("JoinGameGroup", gameKey);
    connection.invoke("NotifyGameUpdated", gameKey);
});


connection.on("ReceivePlayerNames", (whiteName, blackName, isWaiting) => {

    document.getElementById("waitingOverlay").style.display = isWaiting ? "flex" : "none";

    document.getElementById("playerWhite").textContent = whiteName;
    document.getElementById("playerBlack").textContent = blackName;
   
});
