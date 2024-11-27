
using MaskTransitions;
using UnityEngine;

namespace UI.Controllers
{
    internal static class MainMenuController
    {
        internal static void StartGame() => TransitionManager.Instance.LoadLevel("MainScene");

        internal static void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
