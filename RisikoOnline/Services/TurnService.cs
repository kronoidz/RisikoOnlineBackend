using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RisikoOnline.Api;
using RisikoOnline.Data;

namespace RisikoOnline.Services
{
    public class TurnService
    {
        private readonly AppDbContext _dbContext;
        
        public TurnService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public int GetAssignedArmies(List<TerritoryOwnership> ownerships)
        {
            int result = 0;
            
            // Continent ownership
            foreach (var continent in Continents.Map)
            {
                if (continent.Value
                        .TrueForAll(t => 
                            ownerships.Any(T => T.Territory == t)))
                {
                    result += Continents.Armies[continent.Key];
                }
            }
            
            // Number of owned territories
            result += ownerships.Count / 3;

            return result;
        }
        
        public async Task<bool> ValidateReinforcement(
            List<TerritoryOwnership> oldOwnerships,
            List<TerritoryOwnershipDto> newOwnerships,
            Match match,
            int expectedPlacedArmies)
        {
            if (oldOwnerships.Count != newOwnerships.Count) return false;
            
            await _dbContext.Entry(match).Collection(m => m.PlayerStates).LoadAsync();

            int sum = 0;
            foreach (var newOwnership in newOwnerships)
            {
                var ownership = newOwnership;
                var oldOwnership = oldOwnerships.Find(o => o.Territory == ownership.Territory);
                
                if (oldOwnership == null) return false;

                bool valid =
                    match.PlayerStates.Any(ps => newOwnership.Player == ps.PlayerName) &&
                    match.Id == newOwnership.Match &&
                    newOwnership.Armies > 0 &&
                    oldOwnership.Armies <= newOwnership.Armies;

                if (!valid) return false;

                sum += newOwnership.Armies - oldOwnership.Armies;
            }

            return sum == expectedPlacedArmies;
        }
    }
}