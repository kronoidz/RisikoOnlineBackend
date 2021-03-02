using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RisikoOnline.Data;

namespace RisikoOnline.Services
{
    public class MatchService
    {
        public const int MinPlayers = 3;
        public const int MaxPlayers = 6;
        
        private readonly RandomService _random;

        public MatchService(RandomService random)
        {
            _random = random;
        }

        public async Task<Match> CreateMatch(List<Player> players)
        {
            if (players.Count < MinPlayers || players.Count > MaxPlayers)
            {
                throw new ArgumentException(
                    $"Players must be between {MinPlayers} and {MaxPlayers} to create a match",
                    nameof(players) );
            }
            
            Match match = new Match();

            var playerTargets = players.Select(p => p.Name).ToList();
            var territories = Enum.GetValues<Territory>().ToList();

            match.PlayerStates = new List<PlayerState>();
            
            int remaining = players.Count;

            foreach (var player in players)
            {
                PlayerState state = new PlayerState();
                state.PlayerName = player.Name;

                // ASSIGN MISSION
                state.MissionObjective = await _random.Choose(Enum.GetValues<MissionObjective>());
                if (state.MissionObjective == MissionObjective.DestroyEnemy)
                {
                    var targets = playerTargets.Where(t => t != player.Name).ToList();
                    state.TargetPlayerName = await _random.Choose(targets);
                    playerTargets.Remove(state.TargetPlayerName);
                }
                
                // ASSIGN TERRITORIES
                int nTerritories = (int)Math.Round (
                    territories.Count / (double)remaining,
                    MidpointRounding.ToPositiveInfinity ); // Round up
                
                state.Ownerships = new List<TerritoryOwnership>();

                for (int i = 0; i < nTerritories; i++)
                {
                    var ownership = new TerritoryOwnership
                    {
                        Armies = 0,
                        Territory = await _random.Choose(territories)
                    };
                    
                    state.Ownerships.Add(ownership);
                    territories.Remove(ownership.Territory);
                }
                
                // ASSIGN ARMIES
                state.UnplacedArmies = players.Count * (-5) + 50;

                match.PlayerStates.Add(state);
                
                player.CurrentMatchStates ??= new List<PlayerState>();
                player.CurrentMatchStates.Add(state);
                
                remaining--;
            }

            return match;
        }
    }
}