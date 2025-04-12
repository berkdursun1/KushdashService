namespace playerService.Helper
{
    public static class Helper
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1); // 0 ile n dahil
                (list[k], list[n]) = (list[n], list[k]); // swap
            }
        }
    }
}
