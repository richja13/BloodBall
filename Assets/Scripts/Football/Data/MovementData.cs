using Core.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Football.Data
{
    internal class MovementData
    {
        internal static GameObject SelectedPlayer;

        internal static List<PlayerConfig> BlueTeamPlayers;

        internal static List<PlayerConfig> RedTeamPlayers;

        internal static List<GameObject> TestPlayers;

        internal static GameObject Ball;

        internal static float BasicSpeed;

        internal static bool PlayerHasBall = false;
    }
}
