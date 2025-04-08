using playerService.Model;

namespace playerService.Service.Contracts
{
    public interface IPlayerService
    {
        public void AddPlayer(Player player);
        public IEnumerable<Player> GetPlayers();
        public void AddPlayers(IEnumerable<Player> players);
        public Player? GetPlayerByPlayerId(int id);
        public Player? GetPlayerByIndex(int id);
    }
}
