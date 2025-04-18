using System.Text.Json;
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
            List<string> matchedTeams = new List<string>();
            guessPlayerFromUser.Teams.ForEach(item =>
            {
                if(item == "Fenerbahce")
                {
                    // Do not add
                }
                else
                {
                    matchedTeams.Add(targetPlayer.Teams.Contains(item) ? item : "?");
                }
            });
            return new Guess
            {
                Guessed = new GuessedResult
                {
                    Age = guessPlayerFromUser.Age == targetPlayer.Age ? Constants.Helper.Guess_Number.EXACTLY : guessPlayerFromUser.Age < targetPlayer.Age ? Constants.Helper.Guess_Number.DOWN : Constants.Helper.Guess_Number.UP,
                    Foot = guessPlayerFromUser.Foot == targetPlayer.Foot ,
                    Nationality = guessPlayerFromUser.Nationality.Intersect(targetPlayer.Nationality).Any(),
                    Position = guessPlayerFromUser.Position == targetPlayer.Position ,
                    Teams = matchedTeams,
                    Matchs = guessPlayerFromUser.Matchs == targetPlayer.Matchs ? Constants.Helper.Guess_Number.EXACTLY : guessPlayerFromUser.Matchs < targetPlayer.Matchs ? Constants.Helper.Guess_Number.DOWN : Constants.Helper.Guess_Number.UP,
                    Scores = guessPlayerFromUser.Scores == targetPlayer.Scores ? Constants.Helper.Guess_Number.EXACTLY : guessPlayerFromUser.Scores < targetPlayer.Scores ? Constants.Helper.Guess_Number.DOWN : Constants.Helper.Guess_Number.UP,
                    Asists = guessPlayerFromUser.Asists == targetPlayer.Asists ? Constants.Helper.Guess_Number.EXACTLY : guessPlayerFromUser.Asists < targetPlayer.Asists ? Constants.Helper.Guess_Number.DOWN : Constants.Helper.Guess_Number.UP,
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

        public async Task<IEnumerable<string>> GetTeamsOfPlayer(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"players/{id}/transfers");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = JObject.Parse(await response.Content.ReadAsStringAsync());
                JObject doc = JObject.Parse(jsonString.ToString());
                var transfers = doc["transfers"];
                List<string> clubs = new List<string>();
                foreach (var transfer in transfers)
                {
                    var clubFrom = transfer["clubFrom"]["name"].ToString();
                    var clubTo = transfer["clubTo"]["name"].ToString();
                    if (!clubs.Contains(clubFrom)) { clubs.Add(clubFrom); }
                    if (!clubs.Contains(clubTo)) { clubs.Add(clubTo); }
                }
                return clubs;
            }
            else
            {
                return Enumerable.Empty<string>();
            }
            
        }

        public async Task<Stats> GetStats(int id, int clubId)
        {
            Stats stat = new Stats { Match = 0, Score = 0, Asist = 0 };
            HttpResponseMessage response = await _httpClient.GetAsync($"players/{id}/stats");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return stat;
            }
            else
            {
                var jsonString = JObject.Parse(await response.Content.ReadAsStringAsync());
                JObject doc = JObject.Parse(jsonString.ToString());
                var stats = doc["stats"].ToList();
                stats.ForEach(item => 
                {
                    if (Int32.Parse(item["clubId"].ToString()) == clubId)
                    {
                        stat.Match += item["appearances"] != null ? Int32.Parse(item["appearances"].ToString()) : 0;
                        stat.Score += item["goals"] != null ? Int32.Parse(item["goals"].ToString()) : 0;
                        stat.Asist += item["assists"] != null ? Int32.Parse(item["assists"].ToString()): 0;
                    }
                });
            }
            return stat;
        }

        public GuessedResult InitialGuessPlayer(int index)
        {
            Player real = _playerService.GetPlayerByIndex(index);
            return new GuessedResult
            {
                Age = null,
                Foot = null,
                Nationality = null,
                Position = null,
                Teams = Enumerable.Repeat("?", real.Teams.Count-1).ToList(),
                Matchs = null,
                Scores = null,
                Asists = null,
            };
        }
    }
}
