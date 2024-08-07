using Core.Data;
using Football;
using System;
using UnityEngine;

namespace UI.Views
{
    class UIView : MonoBehaviour
    {
        void Update()
        {
            MatchData.UIScore.text = $"{MatchData.BlueScore}:{MatchData.RedScore}";
            MatchData.Time += (MatchData.MatchStarted) ? Time.deltaTime * 4 : 0;
            var timespan = TimeSpan.FromSeconds(MatchData.Time);
            MatchData.UItime.text = string.Format("{0:00}:{1:00}", timespan.TotalMinutes, timespan.Seconds);

            foreach (var player in FootballViewModel.AllPlayers)
            {
                var data = player.GetComponent<PlayerData>();
                data.HpBar.transform.rotation = Camera.main.transform.rotation;
                data.HpBar.transform.position = new Vector3(data.PlayerPosition.x, data.HpBar.transform.position.y, data.PlayerPosition.z);
            }
        }
    }
}