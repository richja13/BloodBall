using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using Core.Enums;
using Cinemachine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.InputSystem.DualShock;
using MaskTransitions;

namespace Core.Data
{
    public static class MatchData
    {
        public static bool LocalCoop
        {
            get 
            {
                foreach (var device in InputSystem.devices.Where(o => o is DualShockGamepad))
                    return true;

                return false;
            }
        }

        public static float RedScore;

        public static bool MatchStarted;

        public static bool CanScoreGoal = true;

        public static float BlueScore;

        public static bool BallOut 
        {
            get { return _ballOut; } 
            set
            {
                if (value != _ballOut)
                {
                    if(value)
                        TransitionManager.Instance.PlayTransition(2);
                    
                    _ballOut = value;
                }
            }
        }
        public static bool Corner;

        static bool _ballOut;

        public static bool BallOutSequence;

        public static double Timer;

        public static Label UItime;

        public static Label UIScore;

        public static GameObject RedGoal;

        public static GameObject BlueGoal;

        public static ProgressBar RedTeamBar;

        public static ProgressBar BlueTeamBar;

        public static Label RedPlayerName;

        public static Label BluePlayerName;

        public static List<Transform> LeftSpawnPoints;

        public static List<Transform> RightSpawnPoints;

        public static GameObject FieldObject;

        public static bool RedTeamHasBall 
        {
            get { return _redTeamHasBall; }

            set
            {
                _redTeamHasBall = value;
                if (value)
                {
                    BlueTeamHasBall = false;
                    LastBallPossesion = Team.Red;
                }
            }
        }

        static bool _redTeamHasBall;

        public static bool BlueTeamHasBall
        {
            get { return _blueTeamHasBall; }

            set
            {
                _blueTeamHasBall = value;
                if (value)
                {
                    RedTeamHasBall = false;
                    LastBallPossesion = Team.Blue;
                }
            }
        }

        static bool _blueTeamHasBall;

        public static Team LastBallPossesion;

        public static bool CanKickBall;

        public static RectTransform RedIndicator;

        public static RectTransform BlueIndicator;

        public static CinemachineVirtualCamera MainCamera;
    }
}
