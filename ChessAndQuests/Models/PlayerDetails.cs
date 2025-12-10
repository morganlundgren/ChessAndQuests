using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ChessAndQuests.Models
{
    public class PlayerDetails
    {
        public int PlayerId { get; set; }

        [Required (ErrorMessage="Invalid username"), MaxLength(15)]
        public string PlayerUserName { get; set; }
        [Required(ErrorMessage = "Invalid Password")]
        public string PlayerPassword { get; set; }

    }
}
