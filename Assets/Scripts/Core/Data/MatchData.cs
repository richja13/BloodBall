using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using Core.Enums;

namespace Core.Data
{
    public class MatchData
    {
        public static bool localCoop;

        public static float RedScore;

        public static bool MatchStarted;

        public static bool CanScoreGoal = true;

        public static float BlueScore;

        public static double Time;

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
    }
}
