using UnityEngine;
using ActiveRagdoll.Modules;
using Football;
using Football.Data;
using Core.Signal;
using Core.Data;

namespace ActiveRagdoll
{
    /// <summary> Default behaviour of an Active Ragdoll </summary>
    public class DefaultBehaviour : MonoBehaviour
    {
        // Author: Sergio Abreu García | https://sergioabreu.me
        [Header("Modules")]
        [SerializeField] 
        ActiveRagdollModule _activeRagdoll;
        
        [SerializeField] 
        PhysicsModule _physicsModule;
        
        [SerializeField] 
        AnimationModule _animationModule;

        [Header("Movement")]
        bool _enableMovement;
        
        Vector2 _movement;
        PlayerData _playerData;

        [Header("Weapon data")]
        [SerializeField]
        WeaponView _weaponView;

        void OnValidate()
        {
            if (_activeRagdoll == null) _activeRagdoll = GetComponent<ActiveRagdollModule>();
            if (_physicsModule == null) _physicsModule = GetComponent<PhysicsModule>();
            if (_animationModule == null) _animationModule = GetComponent<AnimationModule>();
        }

        void Start()
        {
            _playerData = GetComponent<PlayerData>();
            _playerData.OnWeaponAttack += TriggerAttack;
            _weaponView.Controller = _playerData;
            _playerData.Torso = _activeRagdoll.PhysicalTorso.gameObject;
            _playerData.signal.Get<RagdollSignal>().AddListener(_activeRagdoll.ToggleRagdoll);

            if (_playerData.playerTeam == Core.Enums.Team.Red)
                _physicsModule.TargetDirection = (new(-90, 0, 0));
            else
                _physicsModule.TargetDirection = (new(90, 0, 0));
            
        }

        void Update()
        {
            _enableMovement = _playerData.EnableMovement;

            _movement = new Vector2(_playerData.Movement.x, _playerData.Movement.z);
            UpdateMovement();
        }

        void UpdateMovement()
        {
            if (_movement == Vector2.zero || !_enableMovement)
                _animationModule.Animator.SetBool("moving", false);
            else if(!MatchData.BallOut && _enableMovement)
            {
                _physicsModule.TargetDirection = FootballViewModel.Rotation(_movement);
                _animationModule.Animator.SetBool("moving", true);
            }

            _animationModule.Animator.SetFloat("speed", _movement.magnitude * _playerData.WalkSpeed);
            _animationModule.Animator.SetFloat("attackSpeed", _weaponView.AttackSpeed);

            if (MatchData.BallOut)
                _activeRagdoll.TorsoRotation(_playerData.PlayerRotation);
        }

        void TriggerAttack() => _animationModule.Animator.SetTrigger("attack");
    }
}
