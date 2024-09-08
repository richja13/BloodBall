using Core.Controllers;
using Core.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core
{
    public static class CoreViewModel
    {
        public static void LoadPowerBar(ProgressBar teamBar, float maxkick, float kickPower) => MatchView.Instance.LoadPowerBar(teamBar, maxkick, kickPower);

        public static bool GenerateRandomProbability(float chance) => MatchController.GenerateRandomProbability(chance);

        public static bool CheckVector(Vector3 playerPos, Vector3 target, float distance) => MatchController.CheckVector(playerPos, target, distance);

        public static void StartMatch() => MatchController.StartMatch();

        public static int SolveQuadratic(float a, float b, float c, out float Root1, out float Root2)
        {
            int x = MatchController.SolveQuadratic(a, b, c, out var root1, out var root2);
            Root1 = root1;
            Root2 = root2;
            return x;
        }
    }
}
