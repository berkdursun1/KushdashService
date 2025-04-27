using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
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
        public IDistributedCache _distributedCache { get; set; }
        const String cacheKey = "PlayerList";
        public TransferMarktController(ITransferMarktService transferMarktService, IPlayerService playerService, IMapper mapper, IDistributedCache distributedCache)
        {
            _transferMarktService = transferMarktService;
            _playerService = playerService;
            _mapper = mapper;
            _distributedCache = distributedCache;
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
        [HttpGet("RealPlayer")]
        public async Task<Player> RealPlayer(int index, int team)
        {
            Player? player = _playerService.GetPlayerByIndex(index, TeamCodes[team]);
            return _playerService.GetPlayerByIndex(index, TeamCodes[team]) ?? new Player();
        }
    }
}
