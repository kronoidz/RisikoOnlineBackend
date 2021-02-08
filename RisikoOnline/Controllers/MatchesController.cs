using System;
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
    [Route("api/matches")]
    public class MatchesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly MatchService _matchService;

        public MatchesController(AppDbContext dbContext, MatchService matchService)
        {
            _dbContext = dbContext;
            _matchService = matchService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<MatchDto>>> GetMatches()
        {
            string myName = User.Identity?.Name;
            
            var myMatches = await _dbContext.Matches
                .Where(m => m.PlayerStates.Any(ps => ps.PlayerName == myName))
                .Include(m => m.PlayerStates)
                .ToListAsync();

            return Ok(myMatches.Select(m => new MatchDto(m)).ToList());
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<MatchDto>> GetMatch(int id)
        {
            string myName = User.Identity?.Name;
            
            var match = await _dbContext.PlayerStates
                .Where(ps => ps.PlayerName == myName && ps.MatchId == id)
                .Include(ps => ps.Match)
                .ThenInclude(m => m.PlayerStates)
                .Select(ps => new MatchDto(ps.Match))
                .FirstOrDefaultAsync();

            if (match == null)
                return NotFound(new ApiError(ApiErrorType.EntityNotFound));

            return match;
        }
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MatchDto>> CreateMatch()
        {
            string myName = User.Identity?.Name;
            
            var meTask = _dbContext.Players.FindAsync(myName);
            var invitations = await _dbContext.Players
                .Where(p => p.Name == myName)
                .SelectMany(p => p.OutgoingInvitations)
                .Where(i => i.Accepted == true)
                .Include(i => i.Receiver)
                .ToListAsync();

            if (invitations.Count + 1 < MatchService.MinPlayers)
            {
                return UnprocessableEntity(new ApiError(ApiErrorType.NotEnoughInvitations));
            }

            var me = await meTask;
            var players = invitations.Select(i => i.Receiver).Append(me).ToList();
            Match match = await _matchService.CreateMatch(players);

            _dbContext.Add(match);
            _dbContext.RemoveRange(invitations);

            await _dbContext.SaveChangesAsync();
            
            await _dbContext.Entry(match).Navigation("PlayerStates").LoadAsync();

            return Ok(new MatchDto(match));
        }
        
        [HttpGet("{id:int}/ownerships")]
        [Authorize]
        public async Task<ActionResult<List<Api.TerritoryOwnershipDto>>> GetOwnerships(int id)
        {
            string myName = User.Identity?.Name;
            
            // Get match
            var match = await _dbContext.PlayerStates
                .Where(ps => ps.PlayerName == myName && ps.MatchId == id)
                .Select(ps => ps.Match)
                .FirstOrDefaultAsync();

            if (match == null)
                return NotFound(new ApiError(ApiErrorType.EntityNotFound));

            // Requires that the match is fully initialized
            if (string.IsNullOrEmpty(match.CurrentPlayerName))
                return BadRequest(new ApiError(ApiErrorType.MatchNotInitialized));
            
            await _dbContext.Entry(match).Collection(m => m.Ownerships).LoadAsync();

            return match.Ownerships
                .Select(o => new Api.TerritoryOwnershipDto(o))
                .ToList();
        }
    }
}