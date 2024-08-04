using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActiveRagdoll;

/// <summary> Default behaviour of an Active Ragdoll </summary>
public class DefaultBehaviour : MonoBehaviour {
    // Author: Sergio Abreu García | https://sergioabreu.me

    [Header("Modules")]
    [SerializeField] private ActiveRagdoll.ActiveRagdoll _activeRagdoll;
    [SerializeField] private PhysicsModule _physicsModule;
    [SerializeField] private AnimationModule _animationModule;

    [Header("Movement")]
    [SerializeField] private bool _enableMovement = true;
    private Vector2 _movement;

    private Vector3 _aimDirection;

    private void OnValidate() {
        if (_activeRagdoll == null) _activeRagdoll = GetComponent<ActiveRagdoll.ActiveRagdoll>();
        if (_physicsModule == null) _physicsModule = GetComponent<PhysicsModule>();
        if (_animationModule == null) _animationModule = GetComponent<AnimationModule>();
    }

    private void Start() {
        // Link all the functions to its input to define how the ActiveRagdoll will behave.
        // This is a default implementation, where the input player is binded directly to
        // the ActiveRagdoll actions in a very simple way. But any implementation is
        // possible, such as assigning those same actions to the output of an AI system.

        _activeRagdoll.Input.OnMoveDelegates += MovementInput;
        _activeRagdoll.Input.OnMoveDelegates += _physicsModule.ManualTorqueInput;
    }

    private void Update() {
        _movement = GetComponent<TestController>().move;
        UpdateMovement();

        _aimDirection = transform.forward;//
                        //_cameraModule.Camera.transform.forward;

#if UNITY_EDITOR
        // TEST
        if (Input.GetKeyDown(KeyCode.F1))
            Debug.Break();
#endif
    }
    
    private void UpdateMovement() {
        if (_movement == Vector2.zero || !_enableMovement) {
            _animationModule.Animator.SetBool("moving", false);
            return;
        }

        _animationModule.Animator.SetBool("moving", true);
        _animationModule.Animator.SetFloat("speed", _movement.magnitude);        

        float angleOffset = Vector2.SignedAngle(_movement, Vector2.up);
        Vector3 targetForward = Quaternion.AngleAxis(angleOffset, Vector3.up) * Auxiliary.GetFloorProjection(_aimDirection);
        _physicsModule.TargetDirection = targetForward;
    }

    /// <summary> Make the player move and rotate </summary>
    private void MovementInput(Vector2 movement) {
        _movement = movement;
    }
}
