using Core.Enums;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string PlayerNumber;

    public string PlayerName;

    public Transform SpawnPoint;

    public Vector3 Target;

    public float Speed;

    public float MaxKickForce;

    public float Agility;

    public float Durability;

    public PlayerState state;

    public PositionOnField FieldPosition;

    public Team playerTeam;
}
