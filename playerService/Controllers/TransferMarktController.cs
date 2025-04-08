using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using playerService.Dtos.Player;
using playerService.Model;
using playerService.Service.Contracts;

namespace playerService.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TransferMarktController : ControllerBase
    {
        public ITransferMarktService _transferMarktService { get; set; }
        public IPlayerService _playerService { get; set; }
        public IMapper _mapper { get; set; }
        public TransferMarktController(ITransferMarktService transferMarktService, IPlayerService playerService, IMapper mapper)
        {
            _transferMarktService = transferMarktService;
            _playerService = playerService;
            _mapper = mapper;
        }

        [HttpGet("GetAllPlayers")]
        public async Task<IEnumerable<SummaryPlayerDto>> GetAllPlayers() 
        {
            List<SummaryPlayerDto> players = _mapper.Map<List<SummaryPlayerDto>>(_playerService.GetPlayers());
            if(players.Count == 0) 
            {
                for (int i = 2024; i <= 2025; i++)
                {
                    players = players.Concat(_mapper.Map < List < SummaryPlayerDto >> (await _transferMarktService.GetPlayers(i))).ToList();
                }
            }
            return players;
        }
        [HttpGet("Guess")]
        public async Task<Guess> Guess(int playerId, int index)
        {
            return await _transferMarktService.GuessPlayer(playerId, index);
        }
    }
}
