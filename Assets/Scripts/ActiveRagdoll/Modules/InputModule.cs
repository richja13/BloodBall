using UnityEngine;

namespace ActiveRagdoll.Modules
{
    // Author: Sergio Abreu García | https://sergioabreu.me

    /// <summary> Tells the ActiveRagdoll what it should do. Input can be external (like the
    /// one from the player or from another script) and internal (kind of like sensors, such as
    /// detecting if it's on floor). </summary>
    public class InputModule : Module {

        // ---------- INTERNAL INPUT ----------

        [Header("--- FLOOR ---")]
        public float floorDetectionDistance = 0.3f;
        public float maxFloorSlope = 60;

        Rigidbody _rightFoot, _leftFoot;

        void Start() {
            _rightFoot = _activeRagdoll.GetPhysicalBone(HumanBodyBones.RightFoot).GetComponent<Rigidbody>();
            _leftFoot = _activeRagdoll.GetPhysicalBone(HumanBodyBones.LeftFoot).GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Checks whether the given rigidbody is on floor
        /// </summary>
        /// <param name="bodyPart">Part of the body to check</param
        /// <returns> True if the Rigidbody is on floor </returns>
        public bool CheckRigidbodyOnFloor(Rigidbody bodyPart, out Vector3 normal) {
            // Raycast
            Ray ray = new Ray(bodyPart.position, Vector3.down);
            bool onFloor = Physics.Raycast(ray, out RaycastHit info, floorDetectionDistance, ~(1 << bodyPart.gameObject.layer));

            // Additional checks
            onFloor = onFloor && Vector3.Angle(info.normal, Vector3.up) <= maxFloorSlope;

            if (onFloor && info.collider.gameObject.TryGetComponent<Floor>(out Floor floor))
                    onFloor = floor.isFloor;

            normal = info.normal;
            return onFloor;
        }
    }
} 