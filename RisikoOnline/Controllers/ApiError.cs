using System.Collections.Generic;

namespace RisikoOnline.Controllers
{
    public enum ApiErrorType
    {
        InvalidCredentials,
        PlayerNameConflict,
        EntityNotFound,
        InvitationAlreadyAnswered,
        SelfInvitation,
        InvitationReceiverConflict,
        NotEnoughInvitations,
        MatchAlreadyInitialized,
        InvalidMatchInitializationData,
    }
    
    public class ApiError
    {
        private static readonly Dictionary<ApiErrorType, string> Descriptions = new()
        {
            {ApiErrorType.InvalidCredentials, "Invalid credentials"},
            {ApiErrorType.PlayerNameConflict, "A player with that name already exists"},
            {ApiErrorType.EntityNotFound, "The requested entity was not found on the server"},
            {ApiErrorType.InvitationAlreadyAnswered, "The invitation has already been accepted or declined"},
            {ApiErrorType.SelfInvitation, "Attempting to send and invitation to yourself, which is not allowed"},
            {ApiErrorType.InvitationReceiverConflict, "An invitation to that player is pending or accepted"},
            {ApiErrorType.NotEnoughInvitations, "Not enough accepted invitations to create a match"},
            {ApiErrorType.MatchAlreadyInitialized, "Player state in this match was already initialized"},
            {ApiErrorType.InvalidMatchInitializationData, "The provided initialization data object is invalid"}
        };

        public bool IsApiError => true;
        public ApiErrorType Type { get; set; }
        public string Description => Descriptions[Type];
        public string Details { get; set; }

        public ApiError(ApiErrorType type)
        {
            Type = type;
        }

        public ApiError(ApiErrorType type, string details)
            : this(type)
        {
            Details = details;
        }
    }
}
