
namespace UI.Controllers
{
    internal static class MainMenuController
    {
        internal static void StartGame() => MainMenuView.Instance.Load();

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
