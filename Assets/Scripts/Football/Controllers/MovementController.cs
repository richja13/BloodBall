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
        internal static Vector3 Movement(float x, float y, float speed) => new Vector3(x, 0, y) * Time.deltaTime * speed;

        internal static Transform FindClosestPlayer(List<GameObject> players, Transform target, out float distance)
        {
            float smallestDistance = 1000;

            Transform closestPlayer = MovementData.RedSelectedPlayer.transform;

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

            var closestPlayer = FindClosestPlayer(MovementData.AllPlayers, MovementData.Ball.transform, out var distance);
            var data = closestPlayer.GetComponent<PlayerData>();

            if (distance < 2.5f)
            {
                MovementData.PlayerHasBall = true;

                if (data.playerTeam is Team.Red)
                {
                    MovementData.RedSelectedPlayer = closestPlayer.gameObject;
                    MatchData.RedTeamHasBall = true;
                }
                else
                {
                    MovementData.BlueSelectedPlayer = closestPlayer.gameObject;
                    MatchData.BlueTeamHasBall = true;
                }
                MovementData.Ball.transform.SetParent(closestPlayer.transform);

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

        internal static List<GameObject> FieldOfView(GameObject obj, Transform selectedPlayer)
        {
            obj.transform.parent = selectedPlayer;
            obj.transform.localPosition = new Vector3(0, 0.7f, 0);

            float fov = 80f;
            Vector3 origin = selectedPlayer.transform.position;
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
            List<GameObject> fovPlayers = new();

            for (int i = 0; i <= rayCount; i++)
            {
                //Get vector from angle
                Vector3 castDirection = Quaternion.AngleAxis(angle, new Vector3(0, 1, 0)) * (selectedPlayer.transform.forward);

                Vector3 vertex = origin + castDirection * viewDistance;
                vertices[vertexIndex] = vertex;

                RaycastHit[] raycastHits = Physics.RaycastAll(origin, castDirection, viewDistance);
                foreach (var player in raycastHits.Where(players => players.transform.GetComponent<PlayerData>().playerTeam == selectedPlayer.GetComponent<PlayerData>().playerTeam 
                && players.transform != selectedPlayer.transform))
                    fovPlayers.Add(player.transform.gameObject);

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
