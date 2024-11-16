using Core.Data;
using Football;
using System;
using UnityEngine;

namespace UI.Views
{
    internal class UIView : MonoBehaviour
    {
        internal static void CustomUpdate()
        {
            MatchData.UIScore.text = $"{MatchData.BlueScore}:{MatchData.RedScore}";
            MatchData.Time += (MatchData.MatchStarted) ? Time.deltaTime * 4 : 0;
            var timespan = TimeSpan.FromSeconds(MatchData.Time);
            MatchData.UItime.text = string.Format("{0:00}:{1:00}", timespan.TotalMinutes, timespan.Seconds);

            foreach (var data in FootballViewModel.AllPlayers)
            {
                data.HpBar.transform.rotation = Camera.main.transform.rotation;
                data.HpBar.transform.position = new Vector3(data.PlayerPosition.x, data.HpBar.transform.position.y, data.PlayerPosition.z);
            }
        }

        void OnDrawGizmos()
        {
            foreach(PlayerData data in FootballViewModel.AllPlayers)
            {
                if (data.playerTeam == Core.Enums.Team.Red)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.blue;

                Gizmos.DrawLine(data.Target, data.PlayerPosition);
                Gizmos.DrawWireSphere(data.Target, 1f);
            }
        }
    }
}