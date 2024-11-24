using Core.Config;
using Core.Data;
using Football.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Football.Data
{
    internal static class MovementData
    {
        internal static PlayerData RedSelectedPlayer
        {
            get { return _redSelectedPlayer; }

            set
            {
                if (MatchData.RedTeamHasBall)
                {
                    var playertransform = value.Torso.transform;
                    MatchData.MainCamera.Follow = playertransform;
                    MatchData.MainCamera.LookAt = playertransform;

                }
                _redSelectedPlayer = value;
            }
        }

        internal static PlayerData BlueSelectedPlayer
        {
            get { return _blueSelectedPlayer; }

            set 
            {
                if (MatchData.BlueTeamHasBall)
                {
                    var playertransform = value.Torso.transform;
                    MatchData.MainCamera.Follow = playertransform;
                    MatchData.MainCamera.LookAt = playertransform;
                }
                _blueSelectedPlayer = value; 
            }
        }

        static PlayerData _redSelectedPlayer;

        static PlayerData _blueSelectedPlayer;

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
                if (!_playerHasBall && value)
                    AIController.StopRigidbody(Ball.GetComponent<Rigidbody>(), Ball.transform, Ball.transform.position, 100);

                _playerHasBall = value;

                if (!value)
                {
                    foreach (var data in AllPlayers)
                        data.Target = data.PlayerPosition;

                    MatchData.MainCamera.Follow = Ball.transform;
                    MovementController.GetBall();
                }
            }
        }

        static bool _playerHasBall;
    }
}
