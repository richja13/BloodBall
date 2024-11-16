using Core.Data;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Controllers
{
    internal static class MatchController
    {
        internal static bool GenerateRandomProbability(float chance)
        {
            System.Random rand = new();

            return rand.Next(100) > chance;
        }

        internal static bool CheckVector(Vector3 playerPos, Vector3 target, float distance)
        {
            if (Vector3.Distance(playerPos, target) < distance)
                return true;
            else
                return false;
        }

        internal static async void StartMatch()
        {
            await Task.Delay(3000);
            MatchData.MatchStarted = true;
            MatchData.CanKickBall = true;
        }

        internal static Vector3 RandomFieldVector()
        {
            float x = Random.Range(-12, 12);
            float y = Random.Range(-9, 9);
            return new Vector3(x, 15, y);
        }

        internal static int SolveQuadratic(float a, float b, float c, out float root1, out float root2)
        {
            var discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                root1 = Mathf.Infinity;
                root2 = -root1;
                return 0;
            }
            root1 = -b + Mathf.Sqrt(discriminant) / (2 * a);
            root2 = -b - Mathf.Sqrt(discriminant) / (2 * a);
            return discriminant > 0 ? 2 : 1;
        }
    }
}
