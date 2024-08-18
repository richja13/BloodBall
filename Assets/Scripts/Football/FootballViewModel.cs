using Football.Controllers;
using Football.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Football
{
    public static class FootballViewModel
    {
        public static List<PlayerData> AllPlayers { get { return MovementData.AllPlayers; } }

        public static Vector3 Rotation(Transform transform, Vector3 movement) => MovementController.Rotation(transform, movement);
    }
}
