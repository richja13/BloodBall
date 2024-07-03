﻿using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Core.Data
{
    public class MatchData
    {
        public static float RedScore;

        public static float BlueScore;

        public static double Time;

        public static Label UItime;

        public static Label UIScore;

        public static ProgressBar RedTeamBar;

        public static ProgressBar BlueTeamBar;

        public static List<Transform> LeftSpawnPoints;

        public static List<Transform> RightSpawnPoints;
    }
}
