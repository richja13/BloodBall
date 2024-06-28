using Core.Data;
using Football.Data;
using UnityEngine;

namespace Football.Controllers
{
    internal static class SpawnController
    {
        internal static void SpawnPlayers()
        {
            for(int i = 0; i < MatchData.RightSpawnPoints.Count; i++)
            {
                var redPlayer = Object.Instantiate(MovementData.RedTeamPlayers[0].PlayerModel, MatchData.RightSpawnPoints[i]);
                MovementData.RedTeam.Add(redPlayer);

                if(i is 0)
                    MovementData.SelectedPlayer = redPlayer;

                var bluePlayer = Object.Instantiate(MovementData.BlueTeamPlayers[0].PlayerModel, MatchData.LeftSpawnPoints[i]);
                MovementData.BlueTeam.Add(bluePlayer);
            }
        }
    }
}
