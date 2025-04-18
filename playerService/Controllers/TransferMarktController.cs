using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using playerService.Constants;
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
        public IEnumerable<SummaryPlayerDto> GetAllPlayers() 
        {
            List<SummaryPlayerDto> players = _mapper.Map<List<SummaryPlayerDto>>(_playerService.GetPlayers());
            Helper.Helper.Shuffle(players);
            return players;
        }
        [HttpGet("Guess")]
        public async Task<Guess> Guess(int playerId, int index)
        {
            return await _transferMarktService.GuessPlayer(playerId, index);
        }
        [HttpGet("InitialGuess")]
        public async Task<GuessedResult> InitialGuess(int index)
        {
            return _transferMarktService.InitialGuessPlayer(index);
        }
    }
}
