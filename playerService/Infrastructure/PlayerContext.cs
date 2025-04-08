using Microsoft.EntityFrameworkCore;
using playerService.Model;

namespace playerService.Infrastructure
{
    public class PlayerContext : DbContext
    {
        public PlayerContext(DbContextOptions<PlayerContext> options, IConfiguration configuration) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<Player> players { get; set; }

    }
}
