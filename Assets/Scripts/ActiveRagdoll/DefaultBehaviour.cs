using UnityEngine;
using ActiveRagdoll.Modules;
using Football;

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

        PlayerData _playerData;

        [Header("Weapon data")]
        [SerializeField]
        WeaponView _weaponView;

        void OnValidate()
        {
            if (_activeRagdoll == null) _activeRagdoll = GetComponent<ActiveRagdoll>();
            if (_physicsModule == null) _physicsModule = GetComponent<PhysicsModule>();
            if (_animationModule == null) _animationModule = GetComponent<AnimationModule>();
        }

        void Start()
        {
            _playerData = GetComponent<PlayerData>();
            _playerData.OnWeaponAttack += TriggerAttack;
            _weaponView.Controller = _playerData;
            _playerData.Torso = _activeRagdoll.PhysicalTorso.gameObject;
        }

        void Update()
        {
            _movement = new Vector2(_playerData.Movement.x, _playerData.Movement.z);
            _playerData.PlayerPosition = _activeRagdoll.PlayerPosition;
            _playerData.PlayerRotation = _activeRagdoll.PlayerRotation;
            UpdateMovement();
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
            _animationModule.Animator.SetFloat("attackSpeed", _weaponView.AttackSpeed);

            _physicsModule.TargetDirection = FootballViewModel.Rotation(transform, _movement);
        }

        void TriggerAttack() => _animationModule.Animator.SetTrigger("attack");
    }
}
