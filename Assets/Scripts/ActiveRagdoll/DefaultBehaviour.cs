using UnityEngine;
using ActiveRagdoll.Modules;

namespace ActiveRagdoll
{
    /// <summary> Default behaviour of an Active Ragdoll </summary>
    public class DefaultBehaviour : MonoBehaviour
    {
        // Author: Sergio Abreu García | https://sergioabreu.me
        [Header("Modules")]
        [SerializeField] 
        ActiveRagdoll _activeRagdoll;
        
        [SerializeField] 
        PhysicsModule _physicsModule;
        
        [SerializeField] 
        AnimationModule _animationModule;

        [Header("Movement")]
        [SerializeField] 
        bool _enableMovement = true;
        
        Vector2 _movement;

        Vector3 _aimDirection;

        PlayerData _playerData;

        void OnValidate()
        {
            if (_activeRagdoll == null) _activeRagdoll = GetComponent<ActiveRagdoll>();
            if (_physicsModule == null) _physicsModule = GetComponent<PhysicsModule>();
            if (_animationModule == null) _animationModule = GetComponent<AnimationModule>();
        }

        void Start() => _playerData = transform.parent.GetComponent<PlayerData>();

        void Update()
        {
            _movement = -_playerData.Movement;
            UpdateMovement();

            _aimDirection = transform.forward;
        }

        void UpdateMovement()
        {
            if (_movement == Vector2.zero || !_enableMovement)
            {
                _animationModule.Animator.SetBool("moving", false);
                return;
            }

            _animationModule.Animator.SetBool("moving", true);
            _animationModule.Animator.SetFloat("speed", _movement.magnitude);

            float angleOffset = Vector2.SignedAngle(_movement, Vector2.up);
            Vector3 targetForward = Quaternion.AngleAxis(angleOffset, Vector3.up) * Auxiliary.GetFloorProjection(_aimDirection);
            _physicsModule.TargetDirection = targetForward;
        }
    }
}
