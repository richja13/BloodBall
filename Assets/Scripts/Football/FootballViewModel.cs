
using Football.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Football
{
    public static class FootballViewModel
    {
        public static List<GameObject> AllPlayers { get { return MovementData.AllPlayers; } }
    }
}
