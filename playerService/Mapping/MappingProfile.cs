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
            CreateMap<Player,GuessedPlayer>();
        }
    }
}
