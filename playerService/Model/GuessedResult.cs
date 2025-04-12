using static playerService.Constants.Helper;

namespace playerService.Model
{
    public class GuessedResult
    {
        public bool? Position { get; set; }
        public Guess_Number Age { get; set; }
        public bool? Foot { get; set; }
        public bool Nationality { get; set; }
        public List<string> Teams { get; set; }
        public Guess_Number Scores { get; set; }
        public Guess_Number Asists { get; set; }
        public Guess_Number Matchs { get; set; }
    }
}
