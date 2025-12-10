using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace ChessAndQuests.Models
{
    public class GameDetails
    {
        public int GameId { get; set; }
        public int PLayerWhiteId { get; set; }
        public int PlayerBlackId { get; set; }
        [Required(ErrorMessage = "You will have to submit game key.")]
        [StringLength(10, ErrorMessage = "Game Key can be max 10 char.")]
        public string GameKey { get; set; }
        public string CurrentFEN { get; set; }
        public int status { get; set; }
        public int turnId { get; set; }
    }
}
