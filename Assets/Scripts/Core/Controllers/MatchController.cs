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
            await Task.Delay(5000);
            MatchData.MatchStarted = true;
        }

        internal static Vector3 RandomFieldVector()
        {
            float x = Random.Range(-5, 5);
            float y = Random.Range(-3, 3);
            return new Vector3(x, 15, y);
        }
    }
}
