using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RisikoOnline.Entities;

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

        public class InvitationResponse
        {
            public int Id { get; set; }
            public string Sender { get; set; }
            public string Receiver { get; set; }
            public bool? Accepted { get; set; }

            public InvitationResponse(Invitation invitation)
            {
                Id = invitation.Id;
                Sender = invitation.SenderName;
                Receiver = invitation.ReceiverName;
                Accepted = invitation.Accepted;
            }
        }

        public class PostInvitationRequest
        {
            [Required] public string Receiver { get; set; }
        }

        [HttpGet("incoming")]
        [Authorize]
        public async Task<ActionResult<List<InvitationResponse>>> GetIncomingInvitations()
        {
            return await _dbContext.Players
                .Where(p => p.Name == User.Identity.Name)
                .SelectMany(p => p.IncomingInvitations)
                .Select(i => new InvitationResponse(i))
                .ToListAsync();
        }

        [HttpGet("incoming/{id}/accept")]
        [Authorize]
        public async Task<ActionResult<InvitationResponse>> AcceptInvitation(int id)
        {
            Invitation invitation = await _dbContext.Players
                .Where(p => p.Name == User.Identity.Name)
                .SelectMany(p => p.IncomingInvitations)
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();

            if (invitation == null) return NotFound();

            invitation.Accepted = true;
            
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        
        [HttpGet("incoming/{id}/decline")]
        [Authorize]
        public async Task<ActionResult<InvitationResponse>> DeclineInvitation(int id)
        {
            Invitation invitation = await _dbContext.Players
                .Where(p => p.Name == User.Identity.Name)
                .SelectMany(p => p.IncomingInvitations)
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();

            if (invitation == null) return NotFound();

            invitation.Accepted = false;
            
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("outgoing")]
        [Authorize]
        public async Task<ActionResult<List<InvitationResponse>>> GetOutgoingInvitations()
        {
            return await _dbContext.Players
                .Where(p => p.Name == User.Identity.Name)
                .SelectMany(p => p.OutgoingInvitations)
                .Select(i => new InvitationResponse(i))
                .ToListAsync();
        }

        [HttpPost("outgoing")]
        [Authorize]
        public async Task<ActionResult<InvitationResponse>> PostInvitation([FromBody] PostInvitationRequest request)
        {
            if (request.Receiver == User.Identity?.Name)
                return UnprocessableEntity("You can't send an invitation to yourself");
            
            Player receiver = await _dbContext.Players
                .Where(p => p.Name == User.Identity.Name)
                .Include(p => p.IncomingInvitations)
                .FirstOrDefaultAsync();
            
            if (receiver == null) return NotFound();
            
            if (!receiver.IncomingInvitations.TrueForAll(
                i => i.SenderName != User.Identity?.Name || i.Accepted.HasValue))
                return Conflict();

            var invitation = new Invitation
            {
                SenderName = User.Identity?.Name,
                ReceiverName = request.Receiver
            };
            _dbContext.Add(invitation);
            await _dbContext.SaveChangesAsync();

            return new InvitationResponse(invitation);
        }
    }
}