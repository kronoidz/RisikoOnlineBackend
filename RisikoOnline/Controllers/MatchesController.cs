using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RisikoOnline.Data;
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

        public class MatchResponse
        {
            public int Id { get; set; }
            public IEnumerable<string> Players { get; set; }
            
            // null when player states are initializing
            public string CurrentPlayer { get; set; }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<MatchResponse>>> GetMatches()
        {
            string myName = User.Identity?.Name;
            var myMatches = await _dbContext.PlayerStates
                .Where(ps => ps.PlayerName == myName)
                .Select(ps => new MatchResponse
                {
                    Id = ps.MatchId,
                    Players = ps.Match.PlayerStates
                        .Select(ps2 => ps2.PlayerName)
                        .AsEnumerable(),
                    CurrentPlayer = ps.Match.CurrentPlayerName
                })
                .ToListAsync();

            return Ok(myMatches);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<MatchResponse>> GetMatch(int id)
        {
            string myName = User.Identity?.Name;
            var match = await _dbContext.PlayerStates
                .Where(ps => ps.PlayerName == myName && ps.MatchId == id)
                .Select(ps => new MatchResponse
                {
                    Id = ps.MatchId,
                    Players = ps.Match.PlayerStates
                        .Select(ps2 => ps2.PlayerName)
                        .AsEnumerable(),
                    CurrentPlayer = ps.PlayerName
                })
                .FirstOrDefaultAsync();

            if (match == null)
                return NotFound(new ApiError(ApiErrorType.EntityNotFound));

            return match;
        }
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MatchResponse>> CreateMatch()
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

            return Ok(new MatchResponse
            {
                Id = match.Id,
                Players = match.PlayerStates
                    .Select(ps => ps.PlayerName)
                    .AsEnumerable(),
                CurrentPlayer = match.CurrentPlayerName
            });
        }
    }
}