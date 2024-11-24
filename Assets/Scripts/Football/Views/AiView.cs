using UnityEngine;
using Football.Controllers;
using Core.Data;
using Core.Enums;
using Core;
using Football.Data;
using System.Linq;
using static Football.Controllers.AIController;

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
            LoadPlayers();
        }

        internal static void CustomUpdate()
        {
            FieldHalf = CheckFieldHalf();

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

                            if (data.Target == Vector3.zero)
                                data.Target = GenerateRandomVector(data.PlayerPosition, MovementData.Ball.transform.position);

                            if (CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 2))
                                data.Target = GenerateRandomVector(data.PlayerPosition, MovementData.Ball.transform.position);
                            break;

                        case PlayerState.Defence:
                            OnDefence(data);
                            break;

                        case PlayerState.Marking:
                            if (CoreViewModel.CheckVector(data.PlayerPosition, data.Target, 0.2f))
                                MovePlayers(data.Target, data.PlayerPosition, data);
                            break;
                    }
                }
            else
            {
                var redPlayersList = MovementData.AllPlayers.Where(o => o.playerTeam == Team.Red && o.name != MovementData.RedSelectedPlayer.name && !o.KnockedDown).ToList();

                var bluePlayersList = (MatchData.LocalCoop) ? MovementData.AllPlayers.Where(o => o.playerTeam == Team.Blue && o.name != MovementData.BlueSelectedPlayer.name && !o.KnockedDown).ToList() : 
                    MovementData.AllPlayers.Where(o => o.playerTeam == Team.Blue && !o.KnockedDown).ToList();

                PlayerData redPlayer = MovementController.FindClosestPlayer(redPlayersList, MovementData.Ball.transform, out _);
                PlayerData bluePlayer = MovementController.FindClosestPlayer(bluePlayersList, MovementData.Ball.transform, out _);

                redPlayer.state = PlayerState.GetBall;
                redPlayer.CanGetBall = true;
                Rigidbody ballRb = MovementData.Ball.GetComponent<Rigidbody>();
                MovementController.InterceptionDirection(MovementData.Ball.transform.position,
                    redPlayer.PlayerPosition, ballRb.velocity, 15, out var position, out var direction);

                redPlayer.Target = position;

                bluePlayer.state = PlayerState.GetBall;
                bluePlayer.CanGetBall = true;
                ballRb = MovementData.Ball.GetComponent<Rigidbody>();
                MovementController.InterceptionDirection(MovementData.Ball.transform.position,
                    bluePlayer.PlayerPosition, ballRb.velocity, 15, out position, out direction);

                bluePlayer.Target = position;
            }
        }

    }
}
