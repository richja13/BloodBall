using Core.Enums;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    public string PlayerNumber;

    public string PlayerName;

    public Transform SpawnPoint;

    public Transform MarkedPlayer = null;

    public bool Attack;

    public Vector3 Movement;

    public Vector3 Target;

    public float Speed;

    public float MaxKickForce;

    public float Agility;

    public float Durability;

    public PlayerState state;

    public PositionOnField FieldPosition;

    public Team playerTeam;

    public Slider HpBar;
}
