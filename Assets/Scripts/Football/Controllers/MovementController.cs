using Core;
using Core.Data;
using Core.Enums;
using Football.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Football.Controllers
{
    internal static class MovementController
    {
        static bool _canTackle = true;

        internal static Vector3 Movement(float x, float y, float speed) => new Vector3(x, 0, y) * Time.deltaTime * speed;

        internal static PlayerData FindClosestPlayer(List<PlayerData> players, Transform target, out float distance)
        {
            float smallestDistance = 100;

            PlayerData closestPlayer = MovementData.RedSelectedPlayer.GetComponent<PlayerData>();

            foreach (PlayerData player in players)
            {
                if (target != MovementData.Ball.transform)
                    if (target.name == player.name)
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
        }

        internal async static void GetBall()
        {
            if (MovementData.PlayerHasBall)
                return;

            await Task.Delay(500);

            var closestPlayer = FindClosestPlayer(MovementData.AllPlayers, MovementData.Ball.transform, out var distance);

            if (distance < 1f)
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
            }
        }

        /* static int tackletimes = 0;

         internal static bool BallTackle(Transform player)
         {
             if(!CoreViewModel.CheckVector(player.position, MovementData.Ball.transform.position, 1f))
                 return false;

             if (_canTackle)
             {
                 tackletimes++;
                 CanTacke();
                 if (CoreViewModel.GenerateRandomProbability(30))
                 {
                     if(player.gameObject.CompareTag("BluePlayer"))
                     {
                         MovementData.BlueSelectedPlayer = player.gameObject;
                         MatchData.BlueTeamHasBall = true;
                         MatchData.RedTeamHasBall = false;
                     }

                     if(player.gameObject.CompareTag("RedPlayer"))
                     {
                         MovementData.RedSelectedPlayer = player.gameObject;
                         MatchData.BlueTeamHasBall = false;
                         MatchData.RedTeamHasBall = true;
                     }
                     MovementData.Ball.transform.SetParent(player.transform);
                 }
                 Debug.Log("Ball tackle + " + tackletimes);
                 return true;
             }

             return false;

         }

         static async void CanTacke()
         {
             _canTackle = false;
             await Task.Delay(600);
             _canTackle = true;
         }*/

        internal static void AttackEnemy(PlayerData data)
        {
            foreach (PlayerData enemy in MovementData.AllPlayers.Where(enemy => enemy.playerTeam != data.playerTeam))
                if (CoreViewModel.CheckVector(data.PlayerPosition, enemy.PlayerPosition, 1f))
                {
                    data.InvokeAttack();
                    //Add rotation towards enemy

                }
        }

        internal static void LoseBall(PlayerData data)
        {
            if (data.playerTeam == Team.Red)
                MatchData.RedTeamHasBall = false;
            else
                MatchData.BlueTeamHasBall = false;

            MovementData.PlayerHasBall = false;
            MovementData.Ball.transform.parent = null;
        }

        internal static Quaternion Rotation(Transform SelectedPlayer, Vector3 movementVector)
        {
            return (movementVector.x != 0 || movementVector.y != 0) ?
                Quaternion.Slerp(SelectedPlayer.rotation, Quaternion.LookRotation(new Vector3(movementVector.x + 0.1f, 0, movementVector.y + 0.1f)), Time.deltaTime * 5) :
                SelectedPlayer.rotation;
        }

        internal static List<PlayerData> FieldOfView(GameObject obj, Transform selectedPlayer)
        {
            var data = selectedPlayer.GetComponent<PlayerData>();
            obj.transform.parent = data.Torso.transform;
            obj.transform.localPosition = new Vector3(0.7f, 0.7f, 0);

            float fov = 120f;
            Vector3 origin = data.PlayerPosition;
            int rayCount = 25;
            float angle = 45f;
            float angleIncrease = fov / rayCount;
            float viewDistance = 40f;

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
    }
}
