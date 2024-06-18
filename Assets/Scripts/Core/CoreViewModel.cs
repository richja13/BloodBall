using Core.Views;
using UnityEngine.UIElements;

namespace Core
{
    public static class CoreViewModel
    {
        public static void LoadPowerBar(ProgressBar teamBar, float maxkick, float kickPower) => MatchView.Instance.LoadPowerBar(teamBar, maxkick, kickPower);
    }
}
