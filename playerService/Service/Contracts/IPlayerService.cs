using playerService.Model;
using static playerService.Constants.Helper;

namespace playerService.Service.Contracts
{
    public interface IPlayerService
    {
        public void AddPlayer(Player player);
        public IEnumerable<Player> GetPlayers(string team);
        public void AddPlayers(IEnumerable<Player> players);
        public Player? GetPlayerByPlayerId(int id);
        public Player? GetPlayerByIndex(int id, string team);
    }
}
