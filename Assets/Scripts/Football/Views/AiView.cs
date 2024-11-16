using UnityEngine;
using Football.Controllers;
using Core.Data;
using Core.Enums;
using Core;
using Football.Data;
using System.Linq;

namespace Football.Views
{
    internal class AiView : MonoBehaviour
    {
        internal static float FieldHalf = -1;
        Vector3 _meshBounds;
        internal static bool Offence = true;

        void Start()
        {
            _meshBounds = MatchData.FieldObject.GetComponent<MeshFilter>().mesh.bounds.size;
            AIController.LoadPlayers();
        }

        internal static void CustomUpdate()
        {
            FieldHalf = AIController.CheckFieldHalf();

            if (MatchData.RedTeamHasBall)
                Offence = true;
            if (MatchData.BlueTeamHasBall)
                Offence = false;

            MovementData.PlayerHasBall = (MatchData.BlueTeamHasBall || MatchData.RedTeamHasBall) ? true : false;

            if (MovementData.PlayerHasBall)
                foreach (var data in MovementData.AllPlayers)
                {
                    if (data.name == MovementData.RedSelectedPlayer.name)
                        continue;

                    if (!MatchData.LocalCoop)
                    {
                        if ((data.name == MovementData.BlueSelectedPlayer.name && MatchData.BlueTeamHasBall))
                            continue;
                    }
                    else
                        if (data.name == MovementData.BlueSelectedPlayer.name)
                        continue;

                    MovementController.AttackEnemy(data);

                    switch (data.state)
                    {
                        case PlayerState.Attack:

                            if (data.FieldPosition != PositionOnField.Back)
                            {
                                if (data.Target == Vector3.zero)
                                    data.Target = AIController.GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position);

                                if (CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 2))
                                    data.Target = AIController.GenerateRandomVector(data.SpawnPoint.position, MovementData.Ball.transform.position);
                                else
                                    AIController.MovePlayers(data.Target, data.PlayerPosition, data);
                            }
                            else
                            {
                                float ballDistance = Vector3.Distance(data.PlayerPosition, MovementData.Ball.transform.position);
                                float extraDistance = (data.playerTeam == Team.Blue) ? -ballDistance : ballDistance;
                                data.Target = new Vector3(data.SpawnPoint.position.x + (extraDistance * FieldHalf), data.SpawnPoint.position.y, data.SpawnPoint.position.z);

                                if (!CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 1))
                                    AIController.MovePlayers(data.Target, data.PlayerPosition, data);
                            }

                        break;

                        case PlayerState.Defence:
                            AIController.OnDefence(data);
                            break;

                        case PlayerState.Marking:
                            if (CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 0.2f))
                                AIController.MovePlayers(data.Target, data.PlayerPosition, data);
                            break;
                    }
                }
            else
            {
                var redPlayersList = MovementData.AllPlayers.Where(o => o.playerTeam == Team.Red && o.name != MovementData.RedSelectedPlayer.name).ToList();

                var bluePlayersList = (MatchData.LocalCoop) ? MovementData.AllPlayers.Where(o => o.playerTeam == Team.Blue && o.name != MovementData.BlueSelectedPlayer.name).ToList() : 
                    MovementData.AllPlayers.Where(o => o.playerTeam == Team.Blue).ToList();

                var redPlayer = MovementController.FindClosestPlayer(redPlayersList, MovementData.Ball.transform, out _);
                var bluePlayer = MovementController.FindClosestPlayer(bluePlayersList, MovementData.Ball.transform, out _);

                redPlayer.state = PlayerState.GetBall;
                Rigidbody ballRb = MovementData.Ball.GetComponent<Rigidbody>();
                MovementController.InterceptionDirection(MovementData.Ball.transform.position,
                    redPlayer.PlayerPosition, ballRb.velocity, 15, out var position, out var direction);

                redPlayer.Target = position;
                AIController.MovePlayers(redPlayer.Target, redPlayer.PlayerPosition, redPlayer);


                bluePlayer.state = PlayerState.GetBall;
                ballRb = MovementData.Ball.GetComponent<Rigidbody>();
                MovementController.InterceptionDirection(MovementData.Ball.transform.position,
                    bluePlayer.PlayerPosition, ballRb.velocity, 15, out position, out direction);

                bluePlayer.Target = position;
                AIController.MovePlayers(redPlayer.Target, bluePlayer.PlayerPosition, bluePlayer);
            }
        }

    }
}
