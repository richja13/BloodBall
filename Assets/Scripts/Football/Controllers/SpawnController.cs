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
                var player = MovementData.RedTeamPlayers[i];
                player.SpawnPoint = MatchData.RightSpawnPoints[i];
                var redPlayer = Object.Instantiate(player.PlayerModel, player.SpawnPoint);
                redPlayer.name = player.PlayerName + " " + player.PlayerNumber;
                MovementData.RedTeam.Add(redPlayer);

                if(i is 0)
                    MovementData.SelectedPlayer = redPlayer;

                player = MovementData.BlueTeamPlayers[i];
                player.SpawnPoint = MatchData.LeftSpawnPoints[i];
                var bluePlayer = Object.Instantiate(player.PlayerModel, player.SpawnPoint);
                MovementData.BlueTeam.Add(bluePlayer);
            }
        }
    }
}
