using Core;
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
    internal static class MovementController
    {
        internal static Vector3 Movement(float x, float y, float speed) => new Vector3(x, 0, y) * Time.deltaTime * speed;

        internal static PlayerData FindClosestPlayer(List<PlayerData> players, Transform target, out float distance)
        {
            float smallestDistance = 100;

            PlayerData closestPlayer = MovementData.RedSelectedPlayer.GetComponent<PlayerData>();

            foreach (PlayerData player in players)
            {
                if (target != MovementData.Ball.transform)
                    if (target.GetComponentInParent<PlayerData>().name == player.name)
                        continue;

                distance = Vector3.Distance(player.PlayerPosition, target.transform.position);

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    closestPlayer = player;
                }
            }

            distance = smallestDistance;
            return closestPlayer;
        }

        internal static void BallAddForce(float power, Vector3 direction)
        {
            MatchData.RedTeamHasBall = false;
            MatchData.BlueTeamHasBall = false;
            MovementData.PlayerHasBall = false;
            MovementData.Ball.transform.parent = null;
            var rigidbody = MovementData.Ball.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(direction * power, ForceMode.Impulse);

            FieldReferenceHolder.BallHitEffect.transform.position = MovementData.Ball.transform.position;
            FieldReferenceHolder.BallHitEffect.Play();
            BallController.BallParticles(MovementData.Ball.GetComponent<BallView>().BallParticles);
        }

        internal async static void GetBall()
        {
            if (MovementData.PlayerHasBall)
                return;

            await Task.Delay(10);

            var closestPlayer = FindClosestPlayer(MovementData.AllPlayers, MovementData.Ball.transform, out var distance);

            if (distance < 0.8f)
            {
                MovementData.PlayerHasBall = true;

                if (closestPlayer.playerTeam is Team.Red)
                {
                    MovementData.RedSelectedPlayer = closestPlayer.gameObject;
                    MatchData.RedTeamHasBall = true;
                    MatchData.BlueTeamHasBall = false;
                }
                else
                {
                    MovementData.BlueSelectedPlayer = closestPlayer.gameObject;
                    MatchData.BlueTeamHasBall = true;
                    MatchData.RedTeamHasBall = false;
                }
                MovementData.Ball.transform.SetParent(closestPlayer.transform.Find("Physical").transform.Find("Player").transform.Find("Torso"));
                AIController.ManageBack();
                AIController.ManageCentre();
                AIController.ManageForward();
            }
        }

        internal static void AttackEnemy(PlayerData data)
        {
            foreach (PlayerData enemy in MovementData.AllPlayers.Where(enemy => enemy.playerTeam != data.playerTeam))
                if (CoreViewModel.CheckVector(data.PlayerPosition, enemy.PlayerPosition, 1.8f))
                {
                    data.InvokeAttack();
                    data.Target = enemy.PlayerPosition;
                }
        }

        internal static void LoseBall(PlayerData data)
        {

            if (data.playerTeam == Team.Red)
            {
                if (MovementData.RedSelectedPlayer.transform != data.transform)
                    return;

                MatchData.RedTeamHasBall = false;
            }
            else
            {
                if (MovementData.BlueSelectedPlayer.transform != data.transform)
                    return;

                MatchData.BlueTeamHasBall = false;
            }

            MovementData.PlayerHasBall = false;
            MovementData.Ball.transform.parent = null;
        }

        internal static Vector3 Rotation(Transform SelectedPlayer, Vector2 movementVector)
        {
            float angleOffset = Vector2.SignedAngle(movementVector, Vector2.up);
            return Quaternion.AngleAxis(angleOffset, Vector3.up) * Vector3.forward;
        }

        internal static List<PlayerData> FieldOfView(GameObject obj, Transform selectedPlayer)
        {
            var data = selectedPlayer.GetComponent<PlayerData>();
            obj.transform.parent = data.Torso.transform;
            obj.transform.localPosition = new Vector3(0.7f, 0.7f, 0);

            float fov = 120f;
            Vector3 origin = data.PlayerPosition;
            int rayCount = 40;
            float angle = 45f;
            float angleIncrease = fov / rayCount;
            float viewDistance = 80f;

            Vector3[] vertices = new Vector3[rayCount + 1 + 1];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[rayCount * 3];

            vertices[0] = origin;

            int vertexIndex = 1;
            int triangleIndex = 0;
            List<PlayerData> fovPlayers = new();

            for (int i = 0; i <= rayCount; i++)
            {
                Vector3 castDirection = Quaternion.AngleAxis(angle, new Vector3(0, 1, 0)) * (data.Torso.transform.forward);

                Vector3 vertex = origin + castDirection * viewDistance;
                vertices[vertexIndex] = vertex;

                var raycastHit = Physics.Raycast(origin, castDirection, out RaycastHit hit , viewDistance, LayerMask.GetMask("Players"));

                if(raycastHit)
                    if(hit.transform.GetComponentInParent<PlayerData>().playerTeam == data.playerTeam && hit.transform.GetComponentInParent<PlayerData>().name != data.name)
                        fovPlayers.Add(hit.transform.GetComponentInParent<PlayerData>());

            if (i > 0)
                {
                    triangles[triangleIndex + 0] = 0;
                    triangles[triangleIndex + 1] = vertexIndex - 1;
                    triangles[triangleIndex + 2] = vertexIndex;
                    triangleIndex += 3;
                }

                vertexIndex++;
                angle -= angleIncrease;
            }

            return fovPlayers;
        }

        internal static bool InterceptionDirection(Vector3 a, Vector3 b, Vector3 vA, float sB,out Vector3 position, out Vector3 result)
        {
            var aToB = b - a;
            var dC = aToB.magnitude;
            var alpha = Vector3.Angle(aToB, vA) * Mathf.Deg2Rad;
            var sA = vA.magnitude;
            var r = sA / sB;

            if(CoreViewModel.SolveQuadratic(1 - r * r, 2 * r * dC * Mathf.Cos(alpha), -(dC*dC), out var root1, out var root2) == 0)
            {
                result = Vector3.zero;
                position = Vector3.zero;
                return false;
            }

            var dA = Mathf.Max(root1, root2);
            var t = dA / sB;
            var c = a + vA * t;
            result = c - b.normalized;
            position = c;
            return true;
        }
    }
}
