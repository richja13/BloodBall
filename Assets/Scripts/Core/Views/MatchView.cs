using UnityEngine;
using UnityEngine.UIElements;
using Core.Controllers;
using System.Reflection;
using System.Threading;
using System;
using MaskTransitions;

namespace Core.Views
{
    public class MatchView : MonoBehaviour
    {
        internal static MatchView Instance;

        void Awake() => Instance = this;

        void Start() => MatchController.StartMatch();

        //void OnEnable() => MatchData.LocalCoop = (Input.GetJoystickNames().Length > 1 ) ? true : false;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                TransitionManager.Instance.LoadLevel("MainScene", 2f);
                TransitionManager.Instance.LoadLevel("MainMenu");
            }
        }

        internal void LoadPowerBar(ProgressBar powerBar, float highValue, float kickForce)
        {
            powerBar.highValue = highValue;
            powerBar.value = kickForce;
        }

        void OnApplicationQuit()
        {
#if UNITY_EDITOR
            var constructor = SynchronizationContext.Current.GetType().GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
            var newContext = constructor.Invoke(new object[] { Thread.CurrentThread.ManagedThreadId });
            SynchronizationContext.SetSynchronizationContext(newContext as SynchronizationContext);
#endif
        }
    }
}
