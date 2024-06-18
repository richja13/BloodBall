using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    internal class MatchView : MonoBehaviour
    {
        public static MatchView Instance;

        void Awake() => Instance = this;   

        void Update()
        {
            MatchData.UIScore.text = $"{MatchData.BlueScore}:{MatchData.RedScore}";
            MatchData.Time += Time.deltaTime * 4;
            var timespan = TimeSpan.FromSeconds(MatchData.Time);
            MatchData.UItime.text = string.Format("{0:00}:{1:00}", timespan.TotalMinutes, timespan.Seconds);
        }

        internal void LoadPowerBar(ProgressBar powerBar, float highValue, float kickForce)
        {
            powerBar.highValue = highValue;
            powerBar.value = kickForce;
        }
    }
}
