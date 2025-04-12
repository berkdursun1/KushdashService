using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using playerService.Dtos.Player;
using playerService.Infrastructure;
using playerService.Model;
using playerService.Service.Contracts;

namespace playerService.Service
{
    public class TransferMarktService : ITransferMarktService
    {
        public HttpClient _httpClient { get; set; }
        public IPlayerService _playerService { get; set; }
        public IMapper _mapper { get; set; }

        public TransferMarktService(HttpClient httpClient, IPlayerService playerService, IMapper mapper) 
        {
            _httpClient = httpClient;
            _playerService = playerService;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Player?>> GetPlayers(int season_id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"clubs/36/players?season_id={season_id}");
            var jsonString = await response.Content.ReadAsStringAsync();
            var players = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            List<Player> playerList = new List<Player>();
            if (players.ContainsKey("players"))
            {
                try
                {
                    playerList = JsonConvert.DeserializeObject<List<Player>>(players["players"].ToString());

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error deserializing JSON: {e.Message}");
                    throw;
                }
            }
            return playerList;
        }
        public async Task<Guess> GuessPlayer(int playerId, int index)
        {
            Player? guessPlayerFromUser = _playerService.GetPlayerByPlayerId(playerId);
            Player? targetPlayer = _playerService.GetPlayerByIndex(index);
            if(guessPlayerFromUser == null || targetPlayer == null) 
            {
                // Handle if the null case
            }
            return new Guess
            {
                Guessed = new GuessedResult
                {
                    Age = guessPlayerFromUser.Age == targetPlayer.Age ? Constants.Helper.Guess_Number.EXACTLY : guessPlayerFromUser.Age < targetPlayer.Age ? Constants.Helper.Guess_Number.DOWN : Constants.Helper.Guess_Number.UP,
                    Foot = guessPlayerFromUser.Foot == targetPlayer.Foot ,
                    Nationality = guessPlayerFromUser.Nationality.Intersect(targetPlayer.Nationality).Any(),
                    Position = guessPlayerFromUser.Position == targetPlayer.Position ,
                },
                guessedPlayer = _mapper.Map<GuessedPlayer>(guessPlayerFromUser)

            };

        }

        public async Task<string> GetImageUrl(int playerId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"players/{playerId}/profile");
            var jsonString = JObject.Parse(await response.Content.ReadAsStringAsync());
            return jsonString["imageUrl"]?.ToString() ?? string.Empty;
        }
    }
}
