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
using static playerService.Constants.Helper;
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

    private async Task updateAges(PlayerContext context) 
    {
        for (int i = 2000; i <= 2024; i++)
        {
            Console.WriteLine("Current i => " + i);
            IEnumerable<Player> seasonPlayer = await _transferMarktService.GetPlayers(i);
            seasonPlayer.ToList().ForEach(item => 
            {
                try
                {
                    context.players.Where(i => i.Id == item.Id).First().Age = item.Age;
                }
                catch (Exception e)
                {

                    ;
                }
            });
        }
        await context.SaveChangesAsync();
    }


    private async Task SeedInitialData(PlayerContext context)
    {
        var currentIndex = 0;
        IEnumerable<Player> players = new List<Player>();
        for (int i = 2024; i <= 2024; i++)
        {
            IEnumerable<Player> seasonPlayer = await _transferMarktService.GetPlayers(i);
            players = players.Concat(seasonPlayer.ToList());
        }
        List<Player> uniquePlayers = players.ToList().GroupBy(x => x.Id).Select(g => g.First()).ToList();
        foreach (var player in context.players)
        {
            var x = uniquePlayers.Find(x => x.Id == player.Id);
            if (x != null)
            {
                uniquePlayers.Remove(x);
            }
        }
        
        int size = uniquePlayers.Count();
        Console.WriteLine($"Uniqee size {size}");
        foreach (var item in uniquePlayers)
        {
            Console.WriteLine($"Current size {currentIndex++}");
            item.ImageUrl = await _transferMarktService.GetImageUrl(item.Id);
            var teams = await _transferMarktService.GetTeamsOfPlayer(item.Id);
            item.Teams = teams.ToList();
            Stats stat = await _transferMarktService.GetStats(item.Id, BESIKTAS_ID);
            item.Scores = stat.Score;
            item.Matchs = stat.Match;
            item.Asists = stat.Asist;
        }
        context.players.AddRange(uniquePlayers);


        await context.SaveChangesAsync();
    }
}