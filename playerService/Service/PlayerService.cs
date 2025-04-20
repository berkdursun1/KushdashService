using Microsoft.EntityFrameworkCore;
using playerService.Infrastructure;
using playerService.Model;
using playerService.Service.Contracts;
using static playerService.Constants.Helper;

namespace playerService.Service
{
    public class PlayerService : IPlayerService
    {
        public PlayerContext _context { get; set; }

        public PlayerService(PlayerContext context) 
        {
            _context = context;
        }
        public void AddPlayer(Player player)
        {
            _context.players.Add(player);
        }

        public IEnumerable<Player> GetPlayers(string team)
        {
            return _context.players.Where(x => x.Teams.Contains(team));
        }

        public void AddPlayers(IEnumerable<Player> players)
        {
            _context.players.AddRange(players);
            _context.SaveChanges();
        }

        public Player? GetPlayerByPlayerId(int id)
        {
            return _context.players.Where(player => player.Id == id).FirstOrDefault();
        }

        public Player? GetPlayerByIndex(int id, string team)
        {
            return _context.players.Where(x => x.Teams.Contains(team)).Skip(id-1).Take(1).FirstOrDefault();
        }
    }
}
