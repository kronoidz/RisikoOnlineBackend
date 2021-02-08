using System.Collections.Generic;
using System.Linq;
using RisikoOnline.Data;

namespace RisikoOnline.Api
{
    public class MatchDto
    {
        public int Id { get; set; }
        public List<string> Players { get; set; }
        public string CurrentPlayer { get; set; }

        public MatchDto(Match entity)
        {
            Id = entity.Id;
            Players = entity.PlayerStates.Select(ps => ps.PlayerName).ToList();
            CurrentPlayer = entity.CurrentPlayerName;
        }
    }
}