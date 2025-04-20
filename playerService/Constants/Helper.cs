namespace playerService.Constants
{
    public class Helper
    {
        public enum Guess_Number
        {
            UP = 2,
            DOWN = 1,
            EXACTLY = 0
        }
        public static int FENER_ID = 36;
        public static int BESIKTAS_ID = 114;
        public static readonly Dictionary<int, string> TeamCodes
        = new Dictionary<int, string>
        {
            { 0, "Fenerbahce" },
            { 1, "Besiktas" }
        };
    }
}
