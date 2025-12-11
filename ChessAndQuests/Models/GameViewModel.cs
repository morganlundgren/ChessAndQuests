namespace ChessAndQuests.Models
{
    public class GameViewModel
    {
        public string GameKey { get; set; }
        public string PlayerWhiteName { get; set; }
        public string PlayerBlackName { get; set; }
        public string CurrentFEN { get; set; }
        public GameViewModel() { }
    }
}
