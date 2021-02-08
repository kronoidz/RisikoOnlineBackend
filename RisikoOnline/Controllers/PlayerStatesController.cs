using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RisikoOnline.Data;
using RisikoOnline.Api;

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

        // Get my player state (without ownerships)
        [HttpGet("{matchId:int}")]
        [Authorize]
        public async Task<ActionResult<PlayerStateDto>> GetPlayerState(int matchId)
        {
            string myName = User.Identity?.Name;
            var playerState = await _dbContext.PlayerStates
                .Where(ps => ps.MatchId == matchId && ps.PlayerName == myName)
                .Select(ps => new PlayerStateDto(ps))
                .FirstOrDefaultAsync();

            if (playerState == null)
                return NotFound(new ApiError(ApiErrorType.EntityNotFound));

            return playerState;
        }

        // Get my ownerships in this match
        [HttpGet("{matchId:int}/ownerships")]
        [Authorize]
        public async Task<ActionResult<List<TerritoryOwnershipDto>>> GetMyOwnerships(int matchId)
        {
            string myName = User.Identity?.Name;

            var playerState = await _dbContext.PlayerStates
                .Where(ps => ps.MatchId == matchId && ps.PlayerName == myName)
                .Include(ps => ps.Ownerships)
                .FirstOrDefaultAsync();

            if (playerState == null)
                return NotFound(new ApiError(ApiErrorType.EntityNotFound, nameof(PlayerState)));

            return playerState.Ownerships
                .Select(o => new TerritoryOwnershipDto(o))
                .ToList();
        }

        // Initialize ownerships at the beginning of a match
        [HttpPost("{matchId:int}/ownerships")]
        [Authorize]
        public async Task<ActionResult> Initialize (
            int matchId,
            [FromBody] List<TerritoryOwnershipDto> ownerships)
        {
            string myName = User.Identity?.Name;

            var playerState = await _dbContext.PlayerStates
                .Where(ps => ps.MatchId == matchId && ps.PlayerName == myName)
                .Include(ps => ps.Ownerships)
                .FirstOrDefaultAsync();
            
            if (playerState == null)
                return NotFound(new ApiError(ApiErrorType.EntityNotFound));
            
            if (playerState.IsInitialized)
                return UnprocessableEntity(new ApiError(ApiErrorType.MatchAlreadyInitialized));

            // Validate provided army configuration
            bool valid =
                
                // All ownerships are relative to me and this match
                ownerships.All(o => o.Player == myName && o.Match == playerState.MatchId) ||
                
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
                return BadRequest(new ApiError(ApiErrorType.InvalidMatchInitializationData));

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
