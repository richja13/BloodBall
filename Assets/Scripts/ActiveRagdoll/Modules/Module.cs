﻿using UnityEngine;

namespace ActiveRagdoll.Modules
{
    // Author: Sergio Abreu García | https://sergioabreu.me

    /// <summary> The active ragdoll functionality is subdivided in modules, so
    /// everything is easier debug and mantain. </summary>
    [RequireComponent(typeof(ActiveRagdoll))]
    public class Module : MonoBehaviour {
        [SerializeField] protected ActiveRagdoll _activeRagdoll;
        public ActiveRagdoll ActiveRagdoll { get { return _activeRagdoll; } }

        private void OnValidate() {
            if (_activeRagdoll == null) {
                if (!TryGetComponent(out _activeRagdoll))
                    Debug.LogWarning("No ActiveRagdoll could be found for this module.");
            }
        }
    }
}