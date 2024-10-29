using Core.Data;
using Core.Enums;
using Football.Data;
using Football.Views;
using System.Threading.Tasks;
using UnityEngine;

namespace Football.Controllers
{
    internal static class BallController
    {
        internal delegate void HitGoal(Team team);
        internal static event HitGoal hitGoal;

        internal static void CheckGoal(Collision other)
        {
            if (other.gameObject.CompareTag("BlueGoal"))
                hitGoal.Invoke(Team.Blue);

            if (other.gameObject.CompareTag("RedGoal"))
                hitGoal.Invoke(Team.Red);
        }

        internal static async void Goal(Team team)
        {
            if (!MatchData.CanScoreGoal)
                return;

            if (team == Team.Red)
                MatchData.RedScore++;
            else
                MatchData.BlueScore++;
            MovementData.Ball.GetComponent<Rigidbody>().velocity /= 5;

            BallView.Instance.GoalExplosion.transform.position = BallView.Instance.transform.position;
            BallView.Instance.GoalExplosion.Play();
            MatchData.CanScoreGoal = false;
            MatchData.CanKickBall = false;
            Time.timeScale = .4f;

            await Task.Delay(2000);

            AIController.RestartMatch();
        }

        internal static void FieldEndHit(Collider other, Transform transform)
        {
            var collisionPoint = other.ClosestPoint(transform.position);
            transform.position = new Vector3(collisionPoint.x, 1, collisionPoint.z);

            PlayerData data = (MatchData.LastBallPossesion == Team.Red) ? MovementData.BlueSelectedPlayer.GetComponent<PlayerData>() : MovementData.RedSelectedPlayer.GetComponent<PlayerData>();

            data.Torso.transform.position = new Vector3(collisionPoint.x, 0.5f, collisionPoint.z);
            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        internal static async void BallParticles(ParticleSystem particles)
        {
            particles.Play();
            await Task.Delay(1000);
            particles.Stop();
        }
    }
}
