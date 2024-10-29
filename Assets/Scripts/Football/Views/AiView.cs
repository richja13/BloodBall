using UnityEngine;
using Football.Controllers;
using Core.Data;
using Core.Enums;
using Core;
using Football.Data;

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

        void Update()
        {
            FieldHalf = AIController.CheckFieldHalf();

            if (MatchData.RedTeamHasBall)
                Offence = true;
            if (MatchData.BlueTeamHasBall)
                Offence = false;

            foreach (var data in MovementData.AllPlayers)
            {
                    if (data.name == MovementData.RedSelectedPlayer.name)
                        continue;

                if (!MatchData.localCoop)
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

                        if (MatchData.BlueTeamHasBall || MatchData.RedTeamHasBall)
                        {
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
                        }
                        else
                        {
                            data.Target = MovementData.Ball.transform.position;
                            data.state = PlayerState.GetBall;
                        }

                        break;

                    case PlayerState.Defence:
                        AIController.OnDefence(data);

                        break;
                    case PlayerState.Marking:
                        if (CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 0.2f))
                            AIController.MovePlayers(data.Target, data.PlayerPosition, data);
                        break;

                    case PlayerState.GetBall:
                        if ((data.playerTeam == Team.Red && !MatchData.RedTeamHasBall) || (data.playerTeam == Team.Blue && !MatchData.BlueTeamHasBall))
                        {
                            Rigidbody ballRb = MovementData.Ball.GetComponent<Rigidbody>();
                            MovementController.InterceptionDirection(MovementData.Ball.transform.position,
                                data.PlayerPosition, ballRb.velocity, 15, out var position, out var direction);

                            data.Target = position;
                            AIController.MovePlayers(position, data.PlayerPosition, data);
                        }
                        break;

                    case PlayerState.Tackle:

                        break;
                }
            }
        }

    }
}
