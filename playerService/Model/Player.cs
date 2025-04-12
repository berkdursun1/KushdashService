using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace playerService.Model
{
    public class Player
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public string? Position { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public List<string> Nationality { get; set; }
        public string? Foot { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> Teams { get; set; }
        public int Scores { get; set; }
        public int Asists { get; set; }
        public int Matchs { get; set; }
    }
}
