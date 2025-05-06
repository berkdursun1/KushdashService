using AutoMapper;
using playerService.Dtos.Player;
using playerService.Model;

namespace playerService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Player, SummaryPlayerDto>();
            CreateMap<PlayerDto, Player>();
            CreateMap<Player, GuessedPlayer>().AfterMap((src, dest) => {
                dest.Teams.RemoveAll(x => x.Contains("Retired") | x.Contains("Without Club"));
            });
        }
    }
}
