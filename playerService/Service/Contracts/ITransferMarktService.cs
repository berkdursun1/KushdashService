using playerService.Model;

namespace playerService.Service.Contracts
{
    public interface ITransferMarktService
    {
        public Task<IEnumerable<Player>> GetPlayers(int id);

        public Task<IEnumerable<string>> GetTeamsOfPlayer(int id);

        public Task<Stats> GetStats(int id, int clubId);

        public Task<Guess> GuessPlayer(int playerId, int index);

        public Task<string> GetImageUrl(int id);
    }
}
