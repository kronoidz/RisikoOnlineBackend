using RisikoOnline.Data;

namespace RisikoOnline.Api
{
    public class PlayerStateDto
    {
        public int Match { get; set; }
        public string Player { get; set; }

        public bool IsInitialized { get; set; }
            
        public MissionObjective MissionObjective { get; set; }
        public string TargetPlayer { get; set; }

        public int ReinforcementPoints { get; set; }
        public int UnplacedArmies { get; set; }

        public PlayerStateDto(PlayerState entity)
        {
            Match = entity.MatchId;
            Player = entity.PlayerName;
            IsInitialized = entity.IsInitialized;
            MissionObjective = entity.MissionObjective;
            TargetPlayer = entity.TargetPlayerName;
            ReinforcementPoints = entity.ReinforcementPoints;
            UnplacedArmies = entity.UnplacedArmies;
        }
    }
}