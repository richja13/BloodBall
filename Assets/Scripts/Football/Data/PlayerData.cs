using Core;
using Core.Config;
using Core.Data;
using Core.Enums;
using Football.Controllers;
using Football.Data;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    public string PlayerNumber;

    public string PlayerName;

    public SignalHub signal = new();

    public Transform SpawnPoint;
    
    public GameObject Torso;

    public Transform MarkedPlayer = null;

    public Vector3 PlayerPosition { get => Torso.transform.position; }

    public Vector3 PlayerRotation;

    public Vector3 Movement;

    public bool EnableMovement = false;

    public Vector3 Target
    { 
        get { return _target; } 

        set 
        { 
            if (value != _target)
            {
                if (!Attack)
                    _target = AIController.CheckTarget(this, value);

                AIController.MovePlayers(_target, PlayerPosition, this);
            }
        }
    }

    public Vector3 _target;

    public bool Attack;

    public bool KnockedDown 
    {
        get { return _knockedDown; } 
        set
        {
            _knockedDown = value;

            if (value)
                CanGetBall = false;

            if(MovementData.RedSelectedPlayer.transform == this.transform && value == true && MatchData.RedTeamHasBall)
                MovementController.LoseBall(this);
            else if(MovementData.BlueSelectedPlayer.transform == this.transform && value == true && MatchData.BlueTeamHasBall)
                MovementController.LoseBall(this);
        }
    }

    bool _knockedDown;

    public bool Dead
    {
        get { return _dead; }

        set 
        {
            if (value != _dead)
            {
                Destroy(HpBar.gameObject);
                MovementData.AllPlayers.Remove(this);
                EnableMovement = false;
                CanGetBall = false;
            }

            _dead = value;
        }
    }

    bool _dead;

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
            Dead = _health <= 0 ? true : false;
        } 
    }

    float _health = 100;

    public delegate void WeaponAttack();

    public event WeaponAttack OnWeaponAttack;

    public void InvokeAttack() => OnWeaponAttack?.Invoke();

    public void InvokeDamage(float damage) => Health -= damage;

    public ParticleSystem HitParticles;

    public WeaponConfig Weapon;

    [DefaultValue(0.2f)]
    public float ExtraReach
    {
        get { return _extraReach; }
        set { _extraReach = value; }
    }

    float _extraReach = .2f;

    public bool CanGetBall = true;

    public async void BallCooldown(int time)
    {
        CanGetBall = false;
        await Task.Delay(time);
        CanGetBall = true;
    }

    public async void DribbleCooldown(int time)
    {
        CanDribble = false;
        await Task.Delay(time);
        CanDribble = true;
    }

    public bool CanDribble = true;

    public IEnumerator CatchBall(Vector3 targetPos)
    {
        var taskCompleted = false;
        var a = 0;
        while (!taskCompleted) 
        {
            yield return new WaitForSeconds(0.02f);
            if(Vector3.Distance(MovementData.Ball.transform.position, PlayerPosition) < 1)
            {
                Movement = Vector3.zero;
                Target = targetPos;
                var ballRb = MovementData.Ball.GetComponent<Rigidbody>();
                ballRb.velocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
                Vector3 movementVector = new Vector3(MovementData.Ball.transform.position.x - PlayerPosition.x, 0, MovementData.Ball.transform.position.z - PlayerPosition.z).normalized;
                Torso.GetComponent<Rigidbody>().AddForce(8 * movementVector, ForceMode.VelocityChange);
                taskCompleted = true;
            }

            if (a >= 100)
                taskCompleted = true;
            else
                a++;
        }
    }
}
