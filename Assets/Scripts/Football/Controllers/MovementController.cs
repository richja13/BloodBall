using Football.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Football.Controllers
{
    internal static class MovementController
    {
        internal static Vector3 Movement(float x, float y, float speed) => new Vector3(x, 0, y) * Time.deltaTime * speed;

        internal static Transform FindClosestPlayer(List<GameObject> players, Transform target, out float distance)
        {
            float smallestDistance = 1000;
            Transform closestPlayer = MovementData.SelectedPlayer.transform;

            foreach (GameObject player in players)
            {
                if (target != MovementData.Ball.transform)
                    if (target.transform == player.transform)
                        continue;

                distance = Vector3.Distance(player.transform.position, target.transform.position);

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    closestPlayer = player.transform;
                }
            }

            distance = smallestDistance;
            return closestPlayer;
        }

        internal static void BallAddForce(float power, Vector3 direction)
        {
            MovementData.PlayerHasBall = false;
            MovementData.Ball.transform.parent = null;
            var rigidbody = MovementData.Ball.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(new Vector3(direction.x, 0.2f, direction.z) * power, ForceMode.Impulse);
        }

        internal async static void GetBall()
        {
            if (MovementData.PlayerHasBall)
                return;

            await Task.Delay(700);

            //List<GameObject> playerModels = new();
            //MovementData.RedTeamPlayers.ForEach(playerModel => { playerModels.Add(playerModel.PlayerModel); });

            var closestPlayer = FindClosestPlayer(MovementData.TestPlayers, MovementData.Ball.transform, out var distance);

            if (distance < 2.5f)
            {
                MovementData.PlayerHasBall = true;
                MovementData.SelectedPlayer = closestPlayer.gameObject;
                MovementData.Ball.transform.SetParent(MovementData.SelectedPlayer.transform);
            }
        }

        internal static float RotationY(float x, float y)
        {
            if (x > 0)
                if (y > 0)
                    return 45;
                else if (y is 0)
                    return 90;
                else
                    return 135;
            else if (x < 0)
                if (y > 0)
                    return 315;
                else if (y is 0)
                    return 270;
                else
                    return 225;
            else
                if (y > 0)
                return 360;
            else if (y is 0)
                return 0;
            else
                return 180;
        }
    }
}
