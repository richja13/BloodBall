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
                MatchData.BlueScore++;
            else
                MatchData.RedScore++;
            MovementData.Ball.GetComponent<Rigidbody>().velocity /= 5;

            BallView.Instance.GoalExplosion.transform.position = BallView.Instance.transform.position;
            BallView.Instance.GoalExplosion.Play();
            MatchData.CanScoreGoal = false;
            MatchData.CanKickBall = false;
            Time.timeScale = .4f;

            await Task.Delay(2000);

            AIController.RestartMatch();
        }

        internal static void FieldEndHit(Vector3 collisionPoint)
        {
            if (MatchData.BallOutSequence)
                return;

            MatchData.BallOutSequence = true;
            PlayerData data = (MatchData.LastBallPossesion == Team.Red) ? MovementData.BlueSelectedPlayer : MovementData.RedSelectedPlayer;
            data.EnableMovement = false;

            int collisionZ = (collisionPoint.z < 0) ? - 22 : 22;
            
            data.PlayerRotation = (collisionPoint.z > 0) ? new(0, 180, 0) : new(0, 0, 0);

            Vector3 ballPos = new Vector3(collisionPoint.x, .2f, collisionZ);
            BallOut(data, collisionPoint, collisionPoint.z > 0, ballPos, MatchData.Corner);
        }

        internal static async void BallOut(PlayerData data, Vector3 collisionPoint, bool FieldTop, Vector3 ballPos, bool corner)
        {
            List<PlayerData> list = MovementData.AllPlayers.Where(p => p.name != data.name).ToList();
            MovementController.DisableMovement(list);
            List<PlayerData> teamList = list.Where(p => p.playerTeam == data.playerTeam).ToList();

            await Task.Delay(1000);

            if (corner)
            {
                var cornerZ = (FieldTop) ? -21.5f : 21.5f;
                collisionPoint = new(collisionPoint.x, collisionPoint.y, cornerZ);
                foreach (PlayerData playerData in MovementData.AllPlayers.Where(p => p.name != data.name && p.FieldPosition != PositionOnField.GoalKeeper))
                    AIController.StopRigidbody(playerData.Torso.GetComponent<Rigidbody>(), playerData.Torso.transform, GenerateNewPosition(collisionPoint.x > 0), 1000);
            }
            else
                for (int i = 0; i < 3; i++)
                    AIController.StopRigidbody(teamList[i].Torso.GetComponent<Rigidbody>(), teamList[i].Torso.transform, GenerateNewPosition(collisionPoint, FieldTop), 1000);

            AIController.StopRigidbody(MovementData.Ball.GetComponent<Rigidbody>(), MovementData.Ball.transform, ballPos, 1000);
            AIController.StopRigidbody(data.Torso.GetComponent<Rigidbody>(), data.Torso.transform, ballPos, 1000);

          if (data.playerTeam == Team.Red)
              MatchData.RedTeamHasBall = true;
          else
              MatchData.BlueTeamHasBall = true;
        }

        internal static bool CheckIfBallOut(Transform transform, out Vector3 collisionPoint)
        {
            if (!MatchData.BallOut)
            {
                if (transform.position.x > 30.5 || transform.position.x < -30.5)
                {
                    if ((transform.position.z > -2.45 && transform.position.z < 2.45f) || transform.position.y > 1.98)
                    {
                        collisionPoint = Vector3.zero;
                        MatchData.Corner = false;
                        return MatchData.BallOut = false;
                    }

                    var cornerX = float.IsNegative(transform.position.x) ? -30f : 30f;

                    collisionPoint = new(cornerX, 0.2f, 0);
                    MatchData.Corner = true;
                    return MatchData.BallOut = true;
                }

                if (transform.position.z > 21.5f || transform.position.z < -21.5f)
                {
                    collisionPoint = transform.position;
                    MatchData.Corner = false;
                    return MatchData.BallOut = true;
                }
            }

            if (!MatchData.Corner)
            {
                if (transform.position.z < 20 && transform.position.z > -20f)
                {
                    collisionPoint = Vector3.zero;
                    MatchData.BallOutSequence = false;
                    MatchData.Corner = false;
                    return MatchData.BallOut = false;
                }
            }
            else
            {
                if (transform.position.x < 31 && transform.position.x > -31 && transform.position.z < 20 && transform.position.z > -20)
                {
                    collisionPoint = Vector3.zero;
                    MatchData.BallOutSequence = false;
                    MatchData.Corner = false;
                    return MatchData.BallOut = false;
                }
            }
            collisionPoint = Vector3.zero;
            return false;
        }

        internal static Vector3 GenerateNewPosition(Vector3 collisionPoint, bool FieldTop)
        {
            float z = FieldTop ? Random.Range(collisionPoint.z - 4, collisionPoint.z - 7) : Random.Range(collisionPoint.z + 4, collisionPoint.z + 7);
            float x = Random.Range(collisionPoint.x + 6, collisionPoint.x - 6);
            return new(x, .5f, z);
        }

        internal static Vector3 GenerateNewPosition(bool rightCorner)
        {
            var Pos = (rightCorner) ? MatchData.RedGoal.transform.position : MatchData.BlueGoal.transform.position;
            float z = Random.Range(Pos.z - 7, Pos.z + 7);
            float x = rightCorner ? Random.Range(Pos.x - 5, Pos.x - 15) : Random.Range(Pos.x + 5, Pos.x + 15);
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
