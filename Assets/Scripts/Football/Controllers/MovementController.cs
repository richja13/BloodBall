using Core;
using Core.Enums;
using Core.Signal;
using Football.Data;
using Football.Views;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static Core.Data.MatchData;
using static Football.Data.MovementData;

namespace Football.Controllers
{
    internal static class MovementController
    {
        internal static bool ShootR = false;
        internal static bool ShootB = false;
        internal static bool PassBall = false;
     

        internal static void InitializeControllers(bool LocalCoop, InputActions InputAction)
        {
            var InputMap = InputAction.GamePlay;

            InputMap.Shoot.canceled += (context) =>
            {
                var team = (context.control.device is Mouse) ? Team.Red : Team.Blue;
                if (team == Team.Red && RedTeamHasBall)
                    ShootR = true;
                else if (team == Team.Red && !RedTeamHasBall)
                    RedSelectedPlayer.InvokeAttack();

                if (LocalCoop)
                {
                    if (team == Team.Blue && BlueTeamHasBall)
                        ShootB = true;
                    else if (team == Team.Blue && !BlueTeamHasBall)
                        BlueSelectedPlayer.InvokeAttack();
                }
            };

            InputMap.Pass.canceled += (context) => PassBall = true;

            InputMap.Change.canceled += (context) =>
            {
                Team team;
                if (LocalCoop)
                    team = (context.control.device is Keyboard) ? Team.Red : Team.Blue;
                else
                    team = Team.Red;

                ChangePlayer(team);
            };
        }

        static void ChangePlayer(Team team)
        {
            PlayerData closestPlayer;
            if (team == Team.Red)
            {
                PlayerData selectedPlayer = RedSelectedPlayer;
                closestPlayer = FindClosestPlayer(AllPlayers.Where(data => data.playerTeam == Team.Red && data != selectedPlayer).ToList(), Ball.transform, !RedTeamHasBall, out var distance);
                RedSelectedPlayer = closestPlayer;
            }
            else
            {
                PlayerData selectedPlayer = BlueSelectedPlayer;
                closestPlayer = FindClosestPlayer(AllPlayers.Where(data => data.playerTeam == Team.Blue && data != selectedPlayer).ToList(), Ball.transform, BlueTeamHasBall, out var distance);
                BlueSelectedPlayer = closestPlayer;
            }
        }

        internal static bool Pass(out Rigidbody rb, out PlayerData closestPlayer, out PlayerData playerData, out float distance)
        {
            PassBall = false;

            var SelectedPlayer = (RedTeamHasBall) ? RedSelectedPlayer : BlueSelectedPlayer;

            Transform selectedTransform = SelectedPlayer.transform;

            var players = FieldOfView((RedTeamHasBall) ? RedFovObject : BlueFovObject, selectedTransform);

            if (players.Count <= 0)
            {
                closestPlayer = null;
                rb = null;
                playerData = SelectedPlayer;
                distance = 0;
                return false;
            }

            playerData = SelectedPlayer;
            closestPlayer = FindClosestPlayer(players, playerData.Torso.transform, out distance);
            rb = closestPlayer.Torso.GetComponent<Rigidbody>();
            return true;
        }

        internal static PlayerData FindClosestPlayer(List<PlayerData> players, Transform target, out float distance)
        {
            float smallestDistance = 100;

            PlayerData closestPlayer = RedSelectedPlayer;

            foreach (PlayerData player in players)
            {
                if (target != Ball.transform)
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

            PlayerData closestPlayer = RedSelectedPlayer;

            foreach (PlayerData player in players)
            {
                if (target != Ball.transform)
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
            if (!CanKickBall)
                return;

            RedTeamHasBall = false;
            BlueTeamHasBall = false;
            Ball.transform.parent = null;
            player.BallCooldown(300);
            var rigidbody = Ball.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(power * direction, ForceMode.Impulse);
            FieldReferenceHolder.BallHitEffect.transform.position = Ball.transform.position;
            FieldReferenceHolder.BallHitEffect.Play();
            BallController.BallParticles(MovementData.Ball.GetComponent<BallView>().BallParticles);
            Signals.Get<BallShootSignal>().Dispatch();
        }

        internal static void GetBall()
        {
            var closestPlayer = FindClosestPlayer(AllPlayers.Where(p => p.CanGetBall).ToList(), Ball.transform, out var distance);
            if (distance < 0.2f + closestPlayer.ExtraReach && (!closestPlayer.KnockedDown || !closestPlayer.Dead))
            {
                if (closestPlayer.playerTeam is Team.Red)
                {
                    RedSelectedPlayer = closestPlayer;
                    RedTeamHasBall = true;
                }
                else
                {
                    BlueSelectedPlayer = closestPlayer;
                    BlueTeamHasBall = true;
                }

                closestPlayer.ExtraReach = (float)CoreViewModel.GetDefaultValue(typeof(PlayerData), nameof(PlayerData.ExtraReach));

                var ballRb = Ball.GetComponent<Rigidbody>();

                if (closestPlayer.Movement == Vector3.zero)
                {
                    ballRb.angularVelocity = Vector3.zero;
                    ballRb.velocity = Vector3.zero;
                }

                if (closestPlayer.name == RedSelectedPlayer.name || closestPlayer.name == BlueSelectedPlayer.name)
                    ballRb.AddForce(2.5f * closestPlayer.Target.normalized, ForceMode.VelocityChange);
                
                closestPlayer.BallCooldown(300);
                
                AIController.ManageBack();
                AIController.ManageCentre(AiView.Offence);
                AIController.ManageForward(AiView.Offence);
            }
        }

        internal static void AttackEnemy(PlayerData data)
        {
            foreach (PlayerData enemy in AllPlayers.Where(enemy => enemy.playerTeam != data.playerTeam))
                if (CoreViewModel.CheckVector(data.PlayerPosition, enemy.PlayerPosition, 1.1f) && (!enemy.KnockedDown || !enemy.Dead))
                {
                    data.InvokeAttack();
                    data.Target = enemy.PlayerPosition;
                }
        }

        internal static void LoseBall(PlayerData data)
        {
            if (data.playerTeam == Team.Red)
            {
                if (RedSelectedPlayer.name != data.name)
                    return;

                RedTeamHasBall = false;
            }
            else
            {
                if (BlueSelectedPlayer.name != data.name)
                    return;

                BlueTeamHasBall = false;
            }

            Ball.transform.parent = null;
            BallAddForce(3, Ball.transform.forward, data);
            data.BallCooldown(500);
        }

        internal static Vector3 Rotation(Vector2 movementVector)
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
            float viewDistance = 15f;

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

        internal static void EnableMovement() => AllPlayers.ForEach(p => p.EnableMovement = true);

        internal static void DisableMovement(List<PlayerData> dataList) => dataList.ForEach(p => p.EnableMovement = false);

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
