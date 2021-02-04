using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RisikoOnline.Data;

namespace RisikoOnline.Controllers
{
    [ApiController]
    [Route("api/states")]
    public class PlayerStatesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public PlayerStatesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public class PlayerStateResponse
        {
            public int Match { get; set; }
            public string Player { get; set; }

            public bool IsInitialized { get; set; }
            
            public MissionObjective MissionObjective { get; set; }
            public string TargetPlayer { get; set; }

            public int ReinforcementPoints { get; set; }
            public int UnplacedArmies { get; set; }
        }

        [HttpGet("{matchId:int}")]
        [Authorize]
        public async Task<ActionResult<PlayerStateResponse>> GetPlayerState(int matchId)
        {
            string myName = User.Identity?.Name;
            var playerState = await _dbContext.PlayerStates
                .Where(ps => ps.MatchId == matchId && ps.PlayerName == myName)
                .Select(ps => new PlayerStateResponse
                {
                    Match = ps.MatchId,
                    Player = ps.PlayerName,
                    IsInitialized = ps.IsInitialized,
                    MissionObjective = ps.MissionObjective,
                    TargetPlayer = ps.TargetPlayerName,
                    ReinforcementPoints = ps.ReinforcementPoints,
                    UnplacedArmies = ps.UnplacedArmies
                })
                .FirstOrDefaultAsync();

            if (playerState == null)
                return NotFound(new ApiError(ApiErrorType.EntityNotFound));

            return playerState;
        }

        // Server -> client (territories)
        public class InitializationData
        {
            // Territories initially assigned to the player randomly
            public IEnumerable<Territory> Territories { get; set; }
        }

        public class OwnershipDto
        {
            public Territory Territory { get; set; }
            public int Armies { get; set; }
        }

        [HttpGet("{matchId:int}/ownerships")]
        [Authorize]
        public async Task<ActionResult<List<OwnershipDto>>> GetOwnedTerritories(int matchId)
        {
            string myName = User.Identity?.Name;

            return await _dbContext.PlayerStates
                .Where(ps => ps.MatchId == matchId && ps.PlayerName == myName)
                .SelectMany(ps => ps.Ownerships)
                .Select(o => new OwnershipDto
                {
                    Territory = o.Territory,
                    Armies = o.Armies,
                })
                .ToListAsync();
        }
        
        [HttpPost("{matchId:int}/init")]
        [Authorize]
        public async Task<ActionResult> Initialize (
            int matchId,
            [FromBody] List<OwnershipDto> ownerships)
        {
            string myName = User.Identity?.Name;

            var playerState = await _dbContext.PlayerStates
                .Where(ps => ps.MatchId == matchId && ps.PlayerName == myName)
                .Include(ps => ps.Ownerships)
                .FirstOrDefaultAsync();
            
            if (playerState.IsInitialized)
            {
                return UnprocessableEntity(new ApiError(ApiErrorType.MatchAlreadyInitialized));
            }
            
            // Validate provided army configuration
            bool valid =
                
                // All territories have at least 1 army
                ownerships.All(o => o.Armies >= 1) &&
                
                // The sum of all armies is the one assigned at match creation
                ownerships.Sum(o => o.Armies) == playerState.UnplacedArmies &&
                
                // The number of submitted ownerships is correct
                ownerships.Count == playerState.Ownerships.Count &&
                
                // Exactly one ownership has been submitted for all territories initially
                // assigned to the player
                playerState.Ownerships
                    .TrueForAll(
                    playerOwnership => ownerships
                        .Count(submittedOwnership =>
                            submittedOwnership.Territory == playerOwnership.Territory) == 1
                    );

            if (!valid)
            {
                return BadRequest(new ApiError(ApiErrorType.InvalidMatchInitializationData));
            }
            
            // Do initialization

            foreach (var ownership in playerState.Ownerships)
            {
                // ReSharper disable once PossibleNullReferenceException
                ownership.Armies = ownerships
                    .Find(o => o.Territory == ownership.Territory)
                    .Armies;
            }

            playerState.UnplacedArmies = 0;
            playerState.IsInitialized = true;

            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
