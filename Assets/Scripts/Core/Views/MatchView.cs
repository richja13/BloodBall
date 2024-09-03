using UnityEngine;
using UnityEngine.UIElements;
using Core.Controllers;
using System.Reflection;
using System.Threading;
using System;

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
