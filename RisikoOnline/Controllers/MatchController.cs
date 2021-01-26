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
    [Route("api/match")]
    public class MatchController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly MatchService _matchService;

        public MatchController(AppDbContext dbContext, MatchService matchService)
        {
            _dbContext = dbContext;
            _matchService = matchService;
        }

        public class MatchResponse
        {
            public int Id { get; set; }
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

            return Ok(new MatchResponse { Id = match.Id });
        }
    }
}