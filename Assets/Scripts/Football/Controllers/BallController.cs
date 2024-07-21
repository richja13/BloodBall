
using Core;
using Core.Data;
using Core.Enums;
using Football.Data;
using System.Threading.Tasks;
using UnityEngine;

namespace Football.Controllers
{
    internal static class BallController
    {
        internal delegate void HitGoal(Team team);
        internal static event HitGoal hitGoal;

        internal static void CheckGoal(Collider other)
        {
            if (other.gameObject.tag == "BlueGoal")
                hitGoal.Invoke(Team.Blue);

            if (other.gameObject.tag == "RedGoal")
                hitGoal.Invoke(Team.Red);
        }

        internal static async void Goal(Team team)
        {
            if (team == Team.Red)
                MatchData.RedScore++;
            else
                MatchData.BlueScore++;
            MovementData.Ball.GetComponent<Rigidbody>().velocity /= 5;

            await Task.Delay(2000);

            AIController.RestartMatch();
        }

        internal static void FieldEndHit(Collider other, Transform transform)
        {
            var collisionPoint = other.ClosestPoint(transform.position);
            transform.position = new Vector3(collisionPoint.x, 1, collisionPoint.z);
            MovementData.RedSelectedPlayer.transform.position = new Vector3(collisionPoint.x, MovementData.RedSelectedPlayer.transform.position.y, collisionPoint.z);
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
