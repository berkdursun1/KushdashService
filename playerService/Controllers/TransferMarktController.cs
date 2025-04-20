using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using playerService.Constants;
using playerService.Dtos.Player;
using playerService.Model;
using playerService.Service.Contracts;
using static playerService.Constants.Helper;

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
        public IEnumerable<SummaryPlayerDto> GetAllPlayers(int team) 
        {
            List<SummaryPlayerDto> players = _mapper.Map<List<SummaryPlayerDto>>(_playerService.GetPlayers(TeamCodes[team]));
            Helper.Helper.Shuffle(players);
            return players;
        }
        [HttpGet("Guess")]
        public async Task<Guess> Guess(int playerId, int index, int team)
        {
            return await _transferMarktService.GuessPlayer(playerId, index, TeamCodes[team]);
        }
        [HttpGet("InitialGuess")]
        public async Task<GuessedResult> InitialGuess(int index, int team)
        {
            return _transferMarktService.InitialGuessPlayer(index, TeamCodes[team]);
        }
    }
}
