using Core.Config;
using Core.Data;
using Football.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Football.Data
{
    internal class MovementData
    {
        internal static GameObject RedSelectedPlayer
        {
            get { return _redSelectedPlayer; }

            set
            {
                if (MatchData.RedTeamHasBall)
                {
                    var playertransform = value.GetComponent<PlayerData>().Torso.transform;
                    MatchData.Camera.Follow = playertransform;
                    MatchData.Camera.LookAt = playertransform;

                }
                _redSelectedPlayer = value;
            }
        }

        internal static GameObject BlueSelectedPlayer
        {
            get { return _blueSelectedPlayer; }

            set 
            {
                if (MatchData.BlueTeamHasBall)
                {
                    var playertransform = value.GetComponent<PlayerData>().Torso.transform;
                    MatchData.Camera.Follow = playertransform;
                    MatchData.Camera.LookAt = playertransform;
                }
                _blueSelectedPlayer = value; 
            }
        }

        static GameObject _redSelectedPlayer;

        static GameObject _blueSelectedPlayer;

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
                    AIController.StopRigidbody(Ball.GetComponent<Rigidbody>(), Ball.transform, Ball.transform.position);

                _playerHasBall = value;

                if (!value)
                {
                    if(_playerHasBall != value)
                        foreach (var data in AllPlayers)
                            data.Movement = data.Movement/10;

                    MatchData.Camera.Follow = Ball.transform;
                    MovementController.GetBall();
                }
            }
        }

        static bool _playerHasBall;
    }
}
