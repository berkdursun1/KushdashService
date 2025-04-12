using static playerService.Constants.Helper;

namespace playerService.Model
{
    public class Guess
    {
        public GuessedResult Guessed { get; set; }
        public GuessedPlayer guessedPlayer { get; set; }
    }
}
