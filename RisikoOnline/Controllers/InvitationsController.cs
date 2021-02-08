using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RisikoOnline.Data;
using RisikoOnline.Api;

namespace RisikoOnline.Controllers
{
    [ApiController]
    [Route("api/invitations")]
    public class InvitationsController : ControllerBase
    {
        private AppDbContext _dbContext;

        public InvitationsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("incoming")]
        [Authorize]
        public async Task<ActionResult<List<InvitationDto>>> GetIncomingInvitations()
        {
            return await _dbContext.Players
                .Where(p => p.Name == User.Identity.Name)
                .SelectMany(p => p.IncomingInvitations)
                .Select(i => new InvitationDto(i))
                .ToListAsync();
        }

        [HttpGet("incoming/{id}/accept")]
        [Authorize]
        public async Task<ActionResult<InvitationDto>> AcceptInvitation(int id)
        {
            Invitation invitation = await _dbContext.Players
                .Where(p => p.Name == User.Identity.Name)
                .SelectMany(p => p.IncomingInvitations)
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();

            if (invitation == null)
                return NotFound(new ApiError(ApiErrorType.EntityNotFound));
            
            if (invitation.Accepted.HasValue)
                return UnprocessableEntity(new ApiError(ApiErrorType.InvitationAlreadyAnswered));

            invitation.Accepted = true;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        
        [HttpGet("incoming/{id}/decline")]
        [Authorize]
        public async Task<ActionResult<InvitationDto>> DeclineInvitation(int id)
        {
            Invitation invitation = await _dbContext.Players
                .Where(p => p.Name == User.Identity.Name)
                .SelectMany(p => p.IncomingInvitations)
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();

            if (invitation == null)
                return NotFound(new ApiError(ApiErrorType.EntityNotFound));
            
            if (invitation.Accepted.HasValue)
                return UnprocessableEntity(new ApiError(ApiErrorType.InvitationAlreadyAnswered));

            invitation.Accepted = false;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("outgoing")]
        [Authorize]
        public async Task<ActionResult<List<InvitationDto>>> GetOutgoingInvitations()
        {
            return await _dbContext.Players
                .Where(p => p.Name == User.Identity.Name)
                .SelectMany(p => p.OutgoingInvitations)
                .Select(i => new InvitationDto(i))
                .ToListAsync();
        }

        [HttpPost("outgoing")]
        [Authorize]
        public async Task<ActionResult<InvitationDto>> PostInvitation([FromBody] PostInvitationRequest request)
        {
            if (request.Receiver == User.Identity?.Name)
                return UnprocessableEntity(new ApiError(ApiErrorType.SelfInvitation));
            
            Player receiver = await _dbContext.Players
                .Where(p => p.Name == request.Receiver)
                .Include(p => p.IncomingInvitations)
                .FirstOrDefaultAsync();
            
            if (receiver == null) return NotFound(new ApiError(ApiErrorType.EntityNotFound));
            
            if (!receiver.IncomingInvitations.TrueForAll(
                i => i.SenderName != User.Identity?.Name || i.Accepted == false))
                return Conflict(new ApiError(ApiErrorType.InvitationReceiverConflict));

            var invitation = new Invitation
            {
                SenderName = User.Identity?.Name,
                ReceiverName = request.Receiver
            };
            _dbContext.Add(invitation);
            await _dbContext.SaveChangesAsync();

            return new InvitationDto(invitation);
        }
    }
}