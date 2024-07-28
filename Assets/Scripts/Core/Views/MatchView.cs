using UnityEngine;
using UnityEngine.UIElements;
using Core.Controllers;

namespace Core.Views
{
    internal class MatchView : MonoBehaviour
    {
        public static MatchView Instance;

        void Awake() => Instance = this;

        private void Start() => MatchController.StartMatch();

        internal void LoadPowerBar(ProgressBar powerBar, float highValue, float kickForce)
        {
            powerBar.highValue = highValue;
            powerBar.value = kickForce;
        }
    }
}
