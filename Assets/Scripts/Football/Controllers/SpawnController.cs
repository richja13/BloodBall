using Core.Data;
using Football.Data;
using UnityEditor.SceneManagement;
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
                PlayerData data = redPlayer.AddComponent<PlayerData>();
                data.PlayerName = player.PlayerName;
                data.PlayerNumber = player.PlayerNumber;
                data.SpawnPoint = player.SpawnPoint;
                data.MaxKickForce = player.MaxKickForce;
                data.Agility = player.Agility;
                data.Durability = player.Durability;
                data.Speed = player.PlayerSpeed;
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
