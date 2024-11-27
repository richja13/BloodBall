using Core.Data;
using Football.Data;
using Football.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Football.Controllers
{
    internal static class SpawnController
    {
        internal static void SpawnPlayers()
        {
            MovementData.RedTeamPlayers.Reverse();
            MovementData.BlueTeamPlayers.Reverse();

            for (int i = 0; i < MatchData.RightSpawnPoints.Count; i++)
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
                data.FieldPosition = player.FieldPosition;
                data.playerTeam = Core.Enums.Team.Red;
                var canvas = redPlayer.transform.Find("Canvas").GetComponent<Canvas>();
                canvas.worldCamera = Camera.main;
                data.HpBar = canvas.transform.Find("Slider").GetComponent<Slider>();
                data.HpBar.maxValue = 100;
                data.Health = 100;
                data.HitParticles = data.transform.Find("HitEffect").GetComponent<ParticleSystem>();
                if (data.FieldPosition == Core.Enums.PositionOnField.GoalKeeper)
                {
                    data.gameObject.layer = LayerMask.NameToLayer("GoalKeeper");
                    foreach(var child in data.gameObject.GetComponentsInChildren<Transform>())
                        child.gameObject.layer = LayerMask.NameToLayer("GoalKeeper");

                    data.gameObject.AddComponent<GoalKeeperTestView>();
                }
                MovementData.AllPlayers.Add(data);
                MovementData.RedTeam.Add(redPlayer);

                if(i is 0)
                    MovementData.RedSelectedPlayer = data;

                player = MovementData.BlueTeamPlayers[i];
                player.SpawnPoint = MatchData.LeftSpawnPoints[i];
                var bluePlayer = Object.Instantiate(player.PlayerModel, player.SpawnPoint);
                bluePlayer.name = player.PlayerName + " " + player.PlayerNumber;
                data = bluePlayer.AddComponent<PlayerData>();
                data.PlayerName = player.PlayerName;
                data.PlayerNumber = player.PlayerNumber;
                data.SpawnPoint = player.SpawnPoint;
                data.MaxKickForce = player.MaxKickForce;
                data.Agility = player.Agility;
                data.Durability = player.Durability;
                data.Speed = player.PlayerSpeed;
                data.FieldPosition = player.FieldPosition;
                data.playerTeam = Core.Enums.Team.Blue;
                canvas = bluePlayer.transform.Find("Canvas").GetComponent<Canvas>();
                canvas.worldCamera = Camera.main;
                data.HpBar = canvas.transform.Find("Slider").GetComponent<Slider>();
                data.HpBar.maxValue = 100;
                data.Health = 100;
                data.HitParticles = data.transform.Find("HitEffect").GetComponent<ParticleSystem>();
                if (data.FieldPosition == Core.Enums.PositionOnField.GoalKeeper)
                {
                    data.gameObject.layer = LayerMask.NameToLayer("GoalKeeper");
                    foreach (var child in data.gameObject.GetComponentsInChildren<Transform>())
                        child.gameObject.layer = LayerMask.NameToLayer("GoalKeeper");

                    data.gameObject.AddComponent<GoalKeeperTestView>();
                }
                MovementData.BlueTeam.Add(bluePlayer);
                MovementData.AllPlayers.Add(data);

                if (i is 0)
                    MovementData.BlueSelectedPlayer = data;
            }
        }
    }
}
