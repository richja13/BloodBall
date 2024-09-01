using Core.Config;
using Core.Enums;
using Football.Controllers;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    public string PlayerNumber;

    public string PlayerName;

    public Transform SpawnPoint;

    public Transform MarkedPlayer = null;

    public Vector3 PlayerPosition;

    public Vector3 PlayerRotation;

    public Vector3 Movement;

    public Vector3 Target
    { 
        get { return _target; } 
        set { if(!Attack) _target = value; } 
    }
    Vector3 _target;

    public GameObject Torso;

    public bool Attack;

    public bool KnockedDown 
    {
        get { return _knockedDown; } 
        set
        {
            if(value == true)
                MovementController.LoseBall(this);

            _knockedDown = value;
        } 
    }

    bool _knockedDown;

    public float Speed;

    public float MaxKickForce;

    public float Agility;

    public float Durability;

    public PlayerState state;

    public PositionOnField FieldPosition;

    public Team playerTeam;

    public Slider HpBar;

    public float Health
    {
        get { return _health; }

        set 
        {
            _health = value;
            HpBar.value = value; 
        } 
    }

    float _health = 100;

    public delegate void WeaponAttack();

    public event WeaponAttack OnWeaponAttack;

    public void InvokeAttack() => OnWeaponAttack?.Invoke();

    public void InvokeDamage(float damage) => Health -= damage;

    public ParticleSystem HitParticles;

    public WeaponConfig Weapon;
}
