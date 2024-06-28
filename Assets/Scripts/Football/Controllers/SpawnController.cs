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
              var player = Object.Instantiate(MovementData.RedTeamPlayers[0].PlayerModel, MatchData.RightSpawnPoints[i]);
                MovementData.TestPlayers.Add(player);
            }
        }
    }
}
