using System;
using UnityEngine;
using UnityEngine.UIElements;
using Core.Data;
using Core.Controllers;

namespace Core.Views
{
    internal class MatchView : MonoBehaviour
    {
        public static MatchView Instance;

        void Awake() => Instance = this;

        private void Start() => MatchController.StartMatch();

        void Update()
        {
            MatchData.UIScore.text = $"{MatchData.BlueScore}:{MatchData.RedScore}";
            MatchData.Time += (MatchData.MatchStarted) ? Time.deltaTime * 4 : 0;
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
