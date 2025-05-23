﻿using System.Numerics;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using playerService.Dtos.Player;
using playerService.Infrastructure;
using playerService.Model;
using playerService.Service.Contracts;
using StackExchange.Redis;
using static playerService.Constants.Helper;

namespace playerService.Service
{
    public enum SQLTYPE
    {
        ID,
        INDEX
    }
    public class TransferMarktService : ITransferMarktService
    {
        public HttpClient _httpClient { get; set; }
        public IPlayerService _playerService { get; set; }
        public IMapper _mapper { get; set; }
        public IDistributedCache _distributedCache { get; set; }
        

        public TransferMarktService(HttpClient httpClient, IPlayerService playerService, IMapper mapper, IDistributedCache distributedCache) 
        {
            _httpClient = httpClient;
            _playerService = playerService;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }
        public async Task<IEnumerable<Player?>> GetPlayers(int season_id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"clubs/{FENER_ID}/players?season_id={season_id}");
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
        public async Task<Guess> GuessPlayer(int playerId, int index, string team)
        {
            var cacheKeyID = $"playerByID-{playerId}";
            var cacheKeyIndex = $"playerByINDEX-{index}";
            Player? guessPlayerFromUser = await GetPlayerByCache(cacheKeyID, playerId, team, SQLTYPE.ID);
            Player? targetPlayer = await GetPlayerByCache(cacheKeyIndex, index, team, SQLTYPE.INDEX); ;
            if (guessPlayerFromUser == null || targetPlayer == null) 
            {
                // Handle if the null case
            }
            List<string> matchedTeams = new List<string>();
            guessPlayerFromUser.Teams.RemoveAll(x => x.Contains(team) || x.Contains("Retired") || x.Contains("Without Club"));
            guessPlayerFromUser.Teams.ForEach(item =>
            {
                if(item.Contains(team))
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
                    Foot = guessPlayerFromUser.Foot == targetPlayer.Foot,
                    Nationality = guessPlayerFromUser.Nationality.Intersect(targetPlayer.Nationality).Any(),
                    Position = guessPlayerFromUser.Position == targetPlayer.Position,
                    Teams = matchedTeams,
                    Matchs = guessPlayerFromUser.Matchs == targetPlayer.Matchs ? Constants.Helper.Guess_Number.EXACTLY : guessPlayerFromUser.Matchs < targetPlayer.Matchs ? Constants.Helper.Guess_Number.DOWN : Constants.Helper.Guess_Number.UP,
                    Scores = guessPlayerFromUser.Scores == targetPlayer.Scores ? Constants.Helper.Guess_Number.EXACTLY : guessPlayerFromUser.Scores < targetPlayer.Scores ? Constants.Helper.Guess_Number.DOWN : Constants.Helper.Guess_Number.UP,
                    Asists = guessPlayerFromUser.Asists == targetPlayer.Asists ? Constants.Helper.Guess_Number.EXACTLY : guessPlayerFromUser.Asists < targetPlayer.Asists ? Constants.Helper.Guess_Number.DOWN : Constants.Helper.Guess_Number.UP,
                },
                guessedPlayer = _mapper.Map<GuessedPlayer>(guessPlayerFromUser)

            }; ;

        }

        private async Task<Player> GetPlayerByCache(string cacheKeyID, int sqlParameter, string team, SQLTYPE type)
        {
            try
            {
                Player? player;
                string? cachedPlayer = await _distributedCache.GetStringAsync(cacheKeyID);
                if (cachedPlayer == null)
                {
                    // Cache miss
                    player = type == SQLTYPE.ID ? _playerService.GetPlayerByPlayerId(sqlParameter) : _playerService.GetPlayerByIndex(sqlParameter, team);
                    await _distributedCache.SetStringAsync(cacheKeyID, JsonConvert.SerializeObject(player), new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) });
                }
                else
                {
                    player = JsonConvert.DeserializeObject<Player>(cachedPlayer);
                }
                return player;
            }
            catch (RedisConnectionException e)
            {
                return type == SQLTYPE.ID ? _playerService.GetPlayerByPlayerId(sqlParameter) : _playerService.GetPlayerByIndex(sqlParameter, team);
                
            }

        }

        public async Task<string> GetImageUrl(int playerId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"players/{playerId}/profile");
                var jsonString = JObject.Parse(await response.Content.ReadAsStringAsync());
                return jsonString["imageUrl"]?.ToString() ?? string.Empty;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
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
                try
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
                            stat.Asist += item["assists"] != null ? Int32.Parse(item["assists"].ToString()) : 0;
                        }
                    });
                    return stat;
                }
                catch (Exception)
                {

                    return stat ;
                }
            }
        }

        public GuessedResult InitialGuessPlayer(int index, string team)
        {
            Player? real = _playerService.GetPlayerByIndex(index, team);
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
