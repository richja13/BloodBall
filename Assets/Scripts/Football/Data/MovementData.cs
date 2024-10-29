using Core.Config;
using Football.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Football.Data
{
    internal class MovementData
    {
        internal static GameObject RedSelectedPlayer;

        internal static GameObject BlueSelectedPlayer;

        internal static GameObject RedFovObject;

        internal static GameObject BlueFovObject;

        internal static List<PlayerConfig> BlueTeamPlayers;

        internal static List<PlayerConfig> RedTeamPlayers;

        internal static List<GameObject> RedTeam = new();

        internal static List<GameObject> BlueTeam = new();

        internal static List<PlayerData> AllPlayers = new();

        internal static List<Transform> PlayersInView;

        internal static GameObject Ball;

        internal static InputActions Input;

        internal static float BasicSpeed;

        internal static bool PlayerHasBall 
        { 
            get { return _playerHasBall; }
            set
            {
                _playerHasBall = value;
                if(value == false)
                    MovementController.GetBall();
            }
        }

        static bool _playerHasBall;
    }
}
