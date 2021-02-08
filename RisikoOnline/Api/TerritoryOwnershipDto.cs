using RisikoOnline.Data;

namespace RisikoOnline.Api
{
    public class TerritoryOwnershipDto
    {
        public string Player { get; set; }
        public int Match { get; set; }
        public Territory Territory { get; set; }
        public int Armies { get; set; }

        public TerritoryOwnershipDto()
        {
            
        }

        public TerritoryOwnershipDto(TerritoryOwnership entity)
        {
            Player = entity.PlayerName;
            Match = entity.MatchId;
            Territory = entity.Territory;
            Armies = entity.Armies;
        }
    }
}