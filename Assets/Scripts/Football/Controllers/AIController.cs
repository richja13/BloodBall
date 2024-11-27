using Core;
using Core.Data;
using Core.Enums;
using Football.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Football.Controllers
{
    internal static class AIController
    {
        static List<PlayerData> _backPlayers = new();
        static List<PlayerData> _centrePlayers = new();
        static List<PlayerData> _forwardPlayers = new();
        static List<PlayerData> _goalKeepers = new();

        internal static void LoadPlayers()
        {
            foreach (var player in MovementData.AllPlayers)
            {
                var fieldPosition = player.FieldPosition;
                if (fieldPosition == PositionOnField.Forward)
                    _forwardPlayers.Add(player);
                else if (fieldPosition == PositionOnField.Centre)
                    _centrePlayers.Add(player);
                else if (fieldPosition == PositionOnField.Back)
                    _backPlayers.Add(player);
                else
                    _goalKeepers.Add(player);
            }
        }

        internal static void OnDefence(PlayerData data)
        {
            data.MarkedPlayer = null;
            List<PlayerData> EnemyPlayers = new();
            PlayerData SelectedEnemy;
            PlayerData closestPlayer;

            if (data.playerTeam == Team.Red)
            {
                foreach (var players in MovementData.BlueTeam)
                    EnemyPlayers.Add(players.GetComponent<PlayerData>());

                SelectedEnemy = MovementData.BlueSelectedPlayer;
            }
            else
            {
                foreach (var players in MovementData.RedTeam)
                    EnemyPlayers.Add(players.GetComponent<PlayerData>());
                SelectedEnemy = MovementData.RedSelectedPlayer;
            }

                if (Vector3.Distance(data.PlayerPosition, MovementData.Ball.transform.position) < 8)
                {
                    closestPlayer = MovementController.FindClosestPlayer(EnemyPlayers, data.Torso.transform, out _);

                    closestPlayer.MarkedPlayer ??= data.transform;

                    if (closestPlayer == SelectedEnemy)
                    {
                        MovementController.InterceptionDirection(closestPlayer.PlayerPosition,
                          data.PlayerPosition, closestPlayer.Torso.GetComponent<Rigidbody>().velocity, 3, out var position, out _);
                        data.Target = new Vector3(position.x, 0, position.z);
                    }
                    else
                    {
                        if (Vector3.Distance(data.PlayerPosition, SelectedEnemy.PlayerPosition) > 10)
                            if (CoreViewModel.CheckVector(data.PlayerPosition, data.SpawnPoint.position, 7) && closestPlayer.MarkedPlayer == data.transform)
                            {
                                data.Target = closestPlayer.PlayerPosition;
                                //data.state = PlayerState.Marking;
                            }
                        else
                            data.state = PlayerState.GetBall;
                    }
                }
                else
                {
                    float ballDistance = Vector3.Distance(data.PlayerPosition, MovementData.Ball.transform.position);
                    float extraDistance = (data.playerTeam == Team.Blue) ? -ballDistance / 5 : ballDistance / 5;
                    data.Target = new Vector3(data.SpawnPoint.position.x + extraDistance, data.SpawnPoint.position.y, data.SpawnPoint.position.z);
                }
        }

        internal static void ManageForward(bool offence)
        {
            foreach (var data in _forwardPlayers)
            {
                if (data.playerTeam == Team.Red)
                    data.state = (offence) ? PlayerState.Defence : PlayerState.Attack;
                else
                    data.state = (offence) ? PlayerState.Attack : PlayerState.Defence;
            }
        }

        internal static void ManageCentre(bool offence)
        {
            foreach (var data in _centrePlayers)
            {
                if (data.playerTeam == Team.Red)
                    data.state = (offence) ? PlayerState.Attack : PlayerState.Defence;
                else
                    data.state = (offence) ? PlayerState.Defence : PlayerState.Attack;
            }
        }

        internal static void ManageBack()
        {
            foreach (var data in _backPlayers)
                data.state = PlayerState.Defence;
        }

        internal static int CheckFieldHalf() => (MovementData.Ball.transform.position.x < 0) ? -1 : 1;

        internal static void MovePlayers(Vector3 target, Vector3 playerPos, PlayerData data) => data.Movement = new Vector3(target.x - playerPos.x, 0, target.z - playerPos.z).normalized;

        internal static bool CheckYPosition(Vector3 playerPos, Vector3 target, float distance)
        {
            if (Mathf.Abs(playerPos.z) - Mathf.Abs(target.z) > distance)
                return true;
            else
                return false;
        }

        internal static Vector3 GenerateRandomVector(Vector3 playerPosition, Vector3 ballPosition)
        {
            Vector3 newVector = new();
            newVector.y = 0;

            float maxHeight = 20;
            float maxWidth = 12;

            if (MatchData.RedTeamHasBall)
            {
                if (Vector3.Distance(playerPosition, ballPosition) > 3 && Vector3.Distance(playerPosition, ballPosition) < 6)
                    newVector.z = ballPosition.z;
                else
                    newVector.z = playerPosition.z;

                newVector.x = Random.Range(ballPosition.x, ballPosition.x - maxWidth);
            }

            if (MatchData.BlueTeamHasBall)
            {
                if (Vector3.Distance(playerPosition, ballPosition) > 3 && Vector3.Distance(playerPosition, ballPosition) < 6)
                    newVector.z = ballPosition.z;
                else
                    newVector.z = maxHeight;

                newVector.x = Random.Range(ballPosition.x, ballPosition.x + maxWidth);
            }

            return newVector;
        }

        internal async static void Dirbble(PlayerData playerData, Rigidbody rb, int direction, Rigidbody BallRb)
        {
            if (!playerData.CanDribble)
                return;
            
            var movementVector = direction * 800 * Time.fixedDeltaTime * rb.transform.right;
            BallRb.transform.parent = rb.transform;
            rb.AddForce(movementVector, ForceMode.VelocityChange);
            BallRb.AddForce(movementVector/5, ForceMode.VelocityChange);
            playerData.DribbleCooldown(2000);
            await Task.Delay(300);
            BallRb.transform.parent = null;
        }

        internal static Vector3 CheckTarget(PlayerData data, Vector3 target)
        {
            float zPos;
            zPos = (target.z >= 21) ? 20 : (target.z <= -21) ? -20 : target.z; 
            return new(target.x, target.y, zPos);
        }

        internal static void RestartMatch()
        {
            foreach (var data in MovementData.AllPlayers)
            {
                Rigidbody playerRb = data.Torso.GetComponent<Rigidbody>();
                Vector3 pos = new(data.SpawnPoint.position.x, 1.5f, data.SpawnPoint.position.z);
                StopRigidbody(playerRb, data.Torso.transform, pos, 1000);
            }

            MovementController.DisableMovement(MovementData.AllPlayers);
            var rb = MovementData.Ball.GetComponent<Rigidbody>();
            StopRigidbody(rb, rb.transform, Vector3.zero, 1000);

            MatchData.RedTeamHasBall = false;
            MatchData.BlueTeamHasBall = false;
            MatchData.CanScoreGoal = true;
            Time.timeScale = 1.0f;
            CoreViewModel.StartMatch();
        }

        internal static async void StopRigidbody(Rigidbody rb, Transform transform, Vector3 pos, int time)
        {
            rb.isKinematic = true;
            transform.position = pos;
            await Task.Delay(time);
            rb.isKinematic = false;
        }
    }
}

