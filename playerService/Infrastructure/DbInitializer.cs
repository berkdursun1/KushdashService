// DbInitializer.cs
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using playerService.Dtos.Player;
using playerService.Infrastructure;
using playerService.Model;
using playerService.Service;
using playerService.Service.Contracts;

public class DbInitializer
{
    public ITransferMarktService _transferMarktService { get; set; }

    public DbInitializer(ITransferMarktService transfermarktService)
    {
        _transferMarktService = transfermarktService;
    }

    public async Task InitializeAsync(PlayerContext context)
    {
        await context.Database.EnsureCreatedAsync();
        if (!context.players.Any())
        {
            await SeedInitialData(context);
        }
    }

    private async Task SeedInitialData(PlayerContext context)
    {
        IEnumerable<Player> players = new List<Player>();
        for (int i = 2023; i <= 2025; i++)
        {
            IEnumerable<Player> seasonPlayer = await _transferMarktService.GetPlayers(i);
            players = players.Concat(seasonPlayer.ToList());
        }
        foreach (var item in players)
        {
            item.ImageUrl = await _transferMarktService.GetImageUrl(item.Id);
        }
        context.players.AddRange(players.ToList().GroupBy(x => x.Id).Select(g => g.First()));


        await context.SaveChangesAsync();
    }
}