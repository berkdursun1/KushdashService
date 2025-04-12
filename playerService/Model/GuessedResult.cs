using static playerService.Constants.Helper;

namespace playerService.Model
{
    public class GuessedResult
    {
        public bool? Position { get; set; }
        public Guess_Number Age { get; set; }
        public bool? Foot { get; set; }
        public bool Nationality { get; set; }
    }
}
