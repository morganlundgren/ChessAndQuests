namespace ChessAndQuests.Models
{
    public class MoveInfoDTO
    {
        public string FromSquare { get; set; }
        public string ToSquare { get; set; }
        public string CurrentFEN { get; set; }
        public int TurnPlayerId { get; set; }
        public bool ExtraTurnGranted { get; set; }
    }
}
