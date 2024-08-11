
using Football.Data;
using System.Collections.Generic;

namespace Football
{
    public static class FootballViewModel
    {
        public static List<PlayerData> AllPlayers { get { return MovementData.AllPlayers; } }
    }
}
