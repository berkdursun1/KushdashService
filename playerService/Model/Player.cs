using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace playerService.Model
{
    [Table("players")]
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? Name { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? Position { get; set; }

        [Column(TypeName = "integer")]
        public int Age { get; set; }

        [Column(TypeName = "text[]")]
        public List<string> Nationality { get; set; } = new List<string>();

        [Column(TypeName = "varchar(255)")]
        public string? Foot { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? ImageUrl { get; set; }

        [Column(TypeName = "text[]")]
        public List<string> Teams { get; set; } = new List<string>();

        [Column(TypeName = "integer")]
        public int? Scores { get; set; }

        [Column(TypeName = "integer")]
        public int? Asists { get; set; }

        [Column(TypeName = "integer")]
        public int? Matchs { get; set; }
    }
}
