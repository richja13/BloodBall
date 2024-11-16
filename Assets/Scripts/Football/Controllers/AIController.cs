using Core;
using Core.Data;
using Core.Enums;
using Football.Data;
using System.Collections.Generic;
using UnityEngine;
using Football.Views;
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

                SelectedEnemy = MovementData.BlueSelectedPlayer.GetComponent<PlayerData>();
            }
            else
            {
                foreach (var players in MovementData.RedTeam)
                    EnemyPlayers.Add(players.GetComponent<PlayerData>());
                SelectedEnemy = MovementData.RedSelectedPlayer.GetComponent<PlayerData>();
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
                        MovePlayers(data.Target, data.PlayerPosition, data);
                    }
                    else
                    {
                        if (Vector3.Distance(data.PlayerPosition, SelectedEnemy.PlayerPosition) > 10)
                            if (CoreViewModel.CheckVector(data.PlayerPosition, data.SpawnPoint.position, 7) && closestPlayer.MarkedPlayer == data.transform)
                            {
                                data.Target = closestPlayer.PlayerPosition;
                                data.state = PlayerState.Marking;
                            }
                        else
                            data.state = PlayerState.GetBall;
                    }
                }
                else
                {
                    float ballDistance = Vector3.Distance(data.PlayerPosition, MovementData.Ball.transform.position);
                    float extraDistance = (data.playerTeam == Team.Blue) ? -ballDistance / 5 : ballDistance / 5;
                    data.Target = new Vector3(data.SpawnPoint.position.x + (extraDistance * AiView.FieldHalf), data.SpawnPoint.position.y, data.SpawnPoint.position.z);

                    MovePlayers(data.Target, data.PlayerPosition, data);
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

        internal static void MovePlayers(Vector3 target, Vector3 playerPos, PlayerData data) => /*data.KnockedDown = false;*/ data.Movement = (MatchData.MatchStarted) ? new Vector3(target.x - playerPos.x, 0, target.z - playerPos.z).normalized : Vector3.zero;

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
            if (MatchData.RedTeamHasBall)
            {
                float maxHeight = playerPosition.z;
                float maxWidth = -12;

                if (Vector3.Distance(playerPosition, ballPosition) > 1 && Vector3.Distance(playerPosition, ballPosition) < 4)
                    newVector.z = ballPosition.z;
                else
                    newVector.z = maxHeight;

                newVector.x = Random.Range(ballPosition.x, maxWidth);
            }

            if (MatchData.BlueTeamHasBall)
            {
                float maxHeight = playerPosition.z;
                float maxWidth = 12;

                if (Vector3.Distance(playerPosition, ballPosition) > 2 && Vector3.Distance(playerPosition, ballPosition) < 10)
                    newVector.z = ballPosition.z;
                else
                    newVector.z = maxHeight;

                newVector.x = Random.Range(ballPosition.x, maxWidth);
            }
            return newVector;
        }

        internal async static void Dirbble(Rigidbody rb, int direction, Rigidbody BallRb)
        {
            var movementVector = direction * 130 * Time.fixedDeltaTime * rb.transform.right;
            BallRb.transform.parent = rb.transform;
            rb.AddForce(movementVector, ForceMode.VelocityChange);
            BallRb.AddForce(movementVector/5, ForceMode.VelocityChange);
            await Task.Delay(300);
            BallRb.transform.parent = null;
        }

        internal static void RestartMatch()
        {
            foreach (var data in MovementData.AllPlayers)
            {
                Rigidbody playerRb = data.Torso.GetComponent<Rigidbody>();
                Vector3 pos = new(data.SpawnPoint.position.x, 1.5f, data.SpawnPoint.position.z);
                StopRigidbody(playerRb, data.Torso.transform, pos);
            }

            MatchData.MatchStarted = false;
            var rb = MovementData.Ball.GetComponent<Rigidbody>();
            StopRigidbody(rb, rb.transform, Vector3.zero);

            MatchData.RedTeamHasBall = false;
            MatchData.BlueTeamHasBall = false;
            MatchData.CanScoreGoal = true;
            Time.timeScale = 1.0f;
            CoreViewModel.StartMatch();
        }

        internal static async void StopRigidbody(Rigidbody rb, Transform transform, Vector3 pos)
        {
            rb.isKinematic = true;
            transform.position = pos;
            await Task.Delay(100);
            rb.isKinematic = false;
            Debug.Log("Rigidbody stopped");
        }
    }
}

