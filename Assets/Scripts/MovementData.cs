using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    internal class MovementData
    {
        internal static GameObject SelectedPlayer;

        internal static List<GameObject> BlueTeamPlayers;

        internal static List<GameObject> RedTeamPlayers;

        internal static GameObject Ball;

        internal static float BasicSpeed;

        internal static bool PlayerHasBall = false;
    }
}
