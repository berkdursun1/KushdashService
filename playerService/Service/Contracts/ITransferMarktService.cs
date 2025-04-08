using playerService.Model;

namespace playerService.Service.Contracts
{
    public interface ITransferMarktService
    {
        public Task<IEnumerable<Player>> GetPlayers(int id);

        public Task<Guess> GuessPlayer(int playerId, int index);


    }
}
