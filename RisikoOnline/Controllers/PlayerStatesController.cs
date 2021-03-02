using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RisikoOnline.Data;
using RisikoOnline.Api;
using RisikoOnline.Services;

namespace RisikoOnline.Controllers
{
    [ApiController]
    [Route("api/states")]
    public class PlayerStatesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly TurnService _turnService;

        public PlayerStatesController(AppDbContext dbContext, TurnService turnService)
        {
            _dbContext = dbContext;
            _turnService = turnService;
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

        // Submit ownerships
        [HttpPost("{matchId:int}/ownerships")]
        [Authorize]
        public async Task<ActionResult> SubmitOwnerships (
            int matchId,
            [FromBody] List<TerritoryOwnershipDto> ownerships)
        {
            string myName = User.Identity?.Name;

            var playerStates = await _dbContext.PlayerStates
                .Where(ps => ps.MatchId == matchId)
                .ToListAsync();

            var myPlayerState = playerStates
                .FirstOrDefault(ps => ps.PlayerName == myName);

            if (myPlayerState == null)
                return NotFound(new ApiError(ApiErrorType.EntityNotFound));

            await _dbContext.Entry(myPlayerState)
                .Collection(ps => ps.Ownerships)
                .LoadAsync();

            // TODO validation
            // if (!_turnService.ValidateReinforcement(myPlayerState.Ownerships, ownerships,
                // matchId, myPlayerState.UnplacedArmies))
                // return BadRequest(new ApiError(ApiErrorType.InvalidOwnerships));

            // VALIDATION SUCCESSFUL
            
            foreach (var ownership in myPlayerState.Ownerships)
            {
                // ReSharper disable once PossibleNullReferenceException
                ownership.Armies = ownerships
                    .Find(o => o.Territory == ownership.Territory)
                    .Armies;
            }

            myPlayerState.UnplacedArmies = 0;
            myPlayerState.IsInitialized = true;
            
            // Check if all player states are initialized
            var match = await _dbContext.Matches.FindAsync(matchId);
            if (match.CurrentPlayerName == null &&
                playerStates.TrueForAll(ps => ps.IsInitialized))
            {
                var firstPlayer = playerStates.First();
                await _dbContext.Entry(firstPlayer)
                    .Collection(ps => ps.Ownerships)
                    .LoadAsync();
                
                // Initialize first turn
                match.CurrentPlayerName = firstPlayer.PlayerName;
                firstPlayer.UnplacedArmies = _turnService.GetAssignedArmies(firstPlayer.Ownerships);
            }

            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
