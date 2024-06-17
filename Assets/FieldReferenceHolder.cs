using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class FieldReferenceHolder : MonoBehaviour
{
    [SerializeField]
    public MovementView MovementView;

    [SerializeField]
    List<GameObject> _blueTeamPlayers;

    [SerializeField]
    List<GameObject> _redTeamPlayers;

    [SerializeField]
    GameObject _selectedPlayer;

    [SerializeField]
    GameObject _ball;

    [SerializeField]
    float _speed;

    void OnEnable()
    {
        MovementData.SelectedPlayer = _selectedPlayer;
        MovementData.RedTeamPlayers = _redTeamPlayers;
        MovementData.Ball = _ball;
        MovementData.BasicSpeed = _speed;
        MovementData.BlueTeamPlayers = _blueTeamPlayers;
    }
}
