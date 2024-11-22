using Core.Data;
using Core.Enums;
using Football.Data;
using Football.Views;
using System.Collections.Generic;
using System.Linq;
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
            if (MatchData.BallOut)
                return;

            MatchData.BallOut = true;
            var collisionPoint = other.ClosestPoint(transform.position);
            PlayerData data = (MatchData.LastBallPossesion == Team.Red) ? MovementData.BlueSelectedPlayer : MovementData.RedSelectedPlayer;
            data.EnableMovement = false;

            int collisionZ = (collisionPoint.z < 0) ? - 21 : 21;
            BallOut(data, collisionPoint, collisionPoint.z > 0);

            transform.position = new Vector3(collisionPoint.x, .5f, collisionZ);
            data.Torso.transform.position = new Vector3(collisionPoint.x, .5f, collisionZ);

            transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        internal static async void BallOut(PlayerData data, Vector3 collisionPoint, bool FieldTop)
        {
            List<PlayerData> list = MovementData.AllPlayers.Where(p => p.name != data.name).ToList();
            MovementController.DisableMovement(list);

            for(int i = 0; i < list.Count; i++)
                list[i].Torso.transform.position = GenerateNewPosition(collisionPoint, FieldTop);

            await Task.Delay(2000);
            MovementController.EnableMovement();
        }

        internal static Vector3 GenerateNewPosition(Vector3 collisionPoint, bool FieldTop)
        {
            float z = FieldTop ? Random.Range(collisionPoint.z, collisionPoint.z - 3) : Random.Range(collisionPoint.z, collisionPoint.z + 3);
            float x = Random.Range(collisionPoint.x + 5, collisionPoint.x - 5);
            return new(x, .5f, z);
        }

        internal static async void BallParticles(ParticleSystem particles)
        {
            particles.Play();
            await Task.Delay(1000);
            particles.Stop();
        }
    }
}
