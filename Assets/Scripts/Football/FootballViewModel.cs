using Football.Controllers;
using Football.Data;
using Football.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Football
{
    public static class FootballViewModel
    {
        public static List<PlayerData> AllPlayers { get { return MovementData.AllPlayers; } }

        public static Vector3 Rotation(Vector3 movement) => MovementController.Rotation(movement);

        public static void CustomUpdate()
        {
            AiView.CustomUpdate();
            BallView.Instance.CustomUpdate();
            MainAIView.Instance.CustomUpdate();
            MovementView.Instance.CustomUpdate();
        }

        public static void CustomFixedUpdate() => MovementView.Instance.CustomFixedUpdate();
    }
}
