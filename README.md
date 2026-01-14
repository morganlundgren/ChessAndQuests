**♟️ QuestChess – Real-Time Multiplayer Chess with Quests & Rewards**

QuestChess is a real-time, multiplayer, server-authoritative chess application built with ASP.NET MVC, SignalR, and SQL Server,
using chess.js and chessboard.js for game logic and UI.
This is not traditional chess. It’s a unique variant where quests and rewards dynamically affect gameplay,
creating new strategic possibilities and ensuring every match is different.

_**Features:**_

  *Real-time multiplayer gameplay with SignalR
	
  *Server-authoritative game logic to ensure fair play

  *Legal move validation using chess.js
	
  *Interactive chessboard UI with chessboard.js
	
  *Persistent game state in SQL Server
	
  *Full FEN-based position tracking (turn, castling rights, en passant, move counters)
	
  *Detection of: Checkmate, Stalemate, Draw conditions
	
  *Unique quest system with tangible in-game effects
	
  *Rewards that alter game mechanics (extra turns, piece visibility, undo moves)
	
  *Game cleanup and state synchronization on game end
	
  *Deployed on Microsoft Azure

  
**🎯 Quest & Reward System**

In QuestChess, quests are integrated into the gameplay. 
Completing a quest grants real in-game rewards that can change the board, reveal threats, give extra moves, or eventually allow undoing moves. 
Only the first player to complete a quest receives the reward.

_**Featured Quests:**_

  *Capture your opponent’s pawn first → See all threatened pieces for 5 moves
		
  *Move the knight for three consecutive turns → Gain an extra turn
		
  *Capture an opponent’s piece first → See all threatened pieces for 5 moves
		
  *Place a new piece in the center (d4, d5, e4, e5) first → Gain an extra turn
		
  *Move the queen at least two squares first → Gain an extra turn
		
  *Move a rook at least two squares horizontally first → See all threatened pieces
		
  *Position a knight to threaten two valuable pieces first → Undo one move
		
  *Capture three opponent pawns first → Undo one 

_**How It Works:**_

  Quests are assigned per game, with only one player able to complete a quest first.
		
  Progress is tracked server-side in real time.
		
  Completing a quest applies rewards directly to the game, impacting turns, piece visibility, or move options.
		
  Players must balance standard chess strategy with quest objectives.
		

**🧱 Tech Stack**

_**Backend:**_

  *C# / ASP.NET MVC
		
  *SignalR for real-time communication
		
  *SQL Server with Entity Framework
		
  *Azure App Service & Azure SQL
  
_**Frontend:**_

  *JavaScript
		
  *chess.js (move validation and game logic)
		
  *chessboard.js (interactive board UI
  *HTML / CSS
		
  
**🏗️ Architecture Overview**

Server-authoritative design: The server tracks the single source of truth for the board and quests.

_**Client-server flow:**_
  *Player makes a move on the client
		
  *chess.js returns the new fen-string
		
  *Move is sent to the GameController
		
  *Game state and quest progress are updated
		
  *Updated state and quest state is broadcast to both players
		
  *Game-ending conditions and quest rewards are applied
  
All rewards and quest effects are validated server-side to prevent cheating.

**🗃️ Database**

_**Stores:**_

  *Active games & players (White / Black)
	
  *Current FEN positions
	
  *Game status (ongoing, finished)
	
  *Quest definitions & progress per player
	
  *Games are removed upon completion

** 🖥️ Running Locally**

_**To run QuestChess on your machine, you need to set up your own database:**_
  1.Clone the repository
	
  2.Create a SQL Server database
	
  3.You can use SQL Server Management Studio or Azure SQL locally
	
  4.Make sure the database user has read/write permissions
	
  5.Configure the connection string in each DAL file. (Not the best solution but it works)
	
  6.Run database scripts to create the required tables (games, players, quests, etc.)
	
  7.Start the application
	
  8.Open two browser sessions to test multiplayer games with quests

**⚠️ Note: The game will not run correctly without a properly configured database. Make sure the schema matches the tables expected by the application.**

**📌 Deployment**

  *Hosted on Microsoft Azure
	
  *Uses Azure App Service for the web application
	
  *Uses Azure SQL Database for persistence

📈 Future Improvements

  *Better quest logic. As for this moment some loopholes can be found to complete certain quests.
	
  *Spectator mode
	
  *Elo system
	
  *Player authentication
	
  *Clock / time control support
	
  *Showcasing taken pieces
