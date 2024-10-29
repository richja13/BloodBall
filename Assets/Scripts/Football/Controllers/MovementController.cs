using Core;
using Core.Data;
using Core.Enums;
using Core.Signal;
using Football.Data;
using Football.Views;
using System.Collections.Generic;
using System.Linq;
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

        internal static PlayerData FindClosestPlayer(List<PlayerData> players, Transform target, bool attack, out float distance)
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
                    if (attack)
                    {
                        if (player.PlayerPosition.x > target.transform.position.x)
                        {
                            smallestDistance = distance;
                            closestPlayer = player;
                        }
                    }
                    else
                    {
                        if (player.PlayerPosition.x < target.transform.position.x)
                        {
                            smallestDistance = distance;
                            closestPlayer = player;
                        }
                    }
                }
            }

            distance = smallestDistance;
            return closestPlayer;
        }

        internal static void BallAddForce(float power, Vector3 direction, PlayerData player)
        {
            if (!MatchData.CanKickBall)
                return;

            MatchData.RedTeamHasBall = false;
            MatchData.BlueTeamHasBall = false;
            MovementData.Ball.transform.parent = null;
            player.BallCooldown(300);
            var rigidbody = MovementData.Ball.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(direction * power, ForceMode.Impulse);
            FieldReferenceHolder.BallHitEffect.transform.position = MovementData.Ball.transform.position;
            FieldReferenceHolder.BallHitEffect.Play();
            BallController.BallParticles(MovementData.Ball.GetComponent<BallView>().BallParticles);
            Signals.Get<BallShootSignal>().Dispatch();
        }

        internal static void GetBall()
        {
            if (MovementData.PlayerHasBall)
                return;

            var closestPlayer = FindClosestPlayer(MovementData.AllPlayers.Where(p => p.CanGetBall).ToList(), MovementData.Ball.transform, out var distance);

            if (distance < 0.8f && (!closestPlayer.KnockedDown || !closestPlayer.Dead))
            {
                if (closestPlayer.playerTeam is Team.Red)
                {
                    MovementData.RedSelectedPlayer = closestPlayer.gameObject;
                    MatchData.RedTeamHasBall = true;
                    MovementData.Ball.transform.SetParent(closestPlayer.Torso.transform);
                }
                else
                {
                    MovementData.BlueSelectedPlayer = closestPlayer.gameObject;
                    MatchData.BlueTeamHasBall = true;
                    MovementData.Ball.transform.SetParent(closestPlayer.Torso.transform);
                }

                AIController.ManageBack();
                AIController.ManageCentre(AiView.Offence);
                AIController.ManageForward(AiView.Offence);
            }
        }

        internal static void AttackEnemy(PlayerData data)
        {
            foreach (PlayerData enemy in MovementData.AllPlayers.Where(enemy => enemy.playerTeam != data.playerTeam))
                if (CoreViewModel.CheckVector(data.PlayerPosition, enemy.PlayerPosition, 1.8f) && (!enemy.KnockedDown || !enemy.Dead))
                {
                    data.InvokeAttack();
                    data.Target = enemy.PlayerPosition;
                }
        }

        internal static void LoseBall(PlayerData data)
        {
            if (data.playerTeam == Team.Red)
            {
                if (MovementData.RedSelectedPlayer.name != data.name)
                    return;

                MatchData.RedTeamHasBall = false;
            }
            else
            {
                if (MovementData.BlueSelectedPlayer.name != data.name)
                    return;

                MatchData.BlueTeamHasBall = false;
            }

            MovementData.Ball.transform.parent = null;
            BallAddForce(8, MovementData.Ball.transform.forward, data);
            data.BallCooldown(500);
        }

        internal static Vector3 Rotation(Transform SelectedPlayer, Vector2 movementVector)
        {
            float angleOffset = Vector2.SignedAngle(movementVector, Vector2.up);
            return Quaternion.AngleAxis(angleOffset, Vector3.up) * Vector3.forward;
        }

        internal static List<PlayerData> FieldOfView(GameObject obj, Transform selectedPlayer)
        {
            var data = selectedPlayer.GetComponent<PlayerData>();
            obj.transform.rotation = Quaternion.Euler(0, data.Torso.transform.eulerAngles.y, 0);
            obj.transform.position = new Vector3(data.PlayerPosition.x, 0.1f, data.PlayerPosition.z);

            float fov = 120f;
            Vector3 origin = data.PlayerPosition;
            int rayCount = 70;
            float angle = 45f;
            float angleIncrease = fov / rayCount;
            float viewDistance = 80f;

            Vector3[] vertices = new Vector3[rayCount + 2];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[rayCount * 3];

            List<PlayerData> fovPlayers = new();

            vertices[0] = origin;
            int vertexIndex = 1;
            int triangleIndex = 0;

                for (int i = 0; i <= rayCount; i++)
                {
                Vector3 castDirection = Quaternion.AngleAxis(angle, new Vector3(0, 1, 0)) * (obj.transform.forward);

                    Vector3 vertex = origin + castDirection * viewDistance;
                    
                    vertices[vertexIndex] = vertex;

                    var raycastHit = Physics.Raycast(origin, castDirection, out RaycastHit hit, viewDistance, LayerMask.GetMask("Players"));

                    if (raycastHit)
                        if (hit.transform.GetComponentInParent<PlayerData>().playerTeam == data.playerTeam && hit.transform.GetComponentInParent<PlayerData>().name != data.name)
                            fovPlayers.Add(hit.transform.GetComponentInParent<PlayerData>());

                    if (i > 0)
                    {
                        triangles[triangleIndex] = 0;
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
