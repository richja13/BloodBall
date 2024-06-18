using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField]
    UIDocument document;

    float _redScore;

    float _blueScore;

    double _time;

    Label _UItime;

    Label _UIScore;

    ProgressBar _redTeamBar;

    ProgressBar _blueTeamBar;

    void OnEnable()
    {
        //Movement Data
        MovementData.SelectedPlayer = _selectedPlayer;
        MovementData.RedTeamPlayers = _redTeamPlayers;
        MovementData.Ball = _ball;
        MovementData.BasicSpeed = _speed;
        MovementData.BlueTeamPlayers = _blueTeamPlayers;

        //MatchData
        MatchData.Time = 0;
        MatchData.UItime = document.rootVisualElement.Q<Label>(className: "time");
        MatchData.UIScore = document.rootVisualElement.Q<Label>(className: "score");
        MatchData.RedTeamBar = document.rootVisualElement.Q<ProgressBar>(name: "RedTeamProgressBar");
        MatchData.BlueTeamBar = document.rootVisualElement.Q<ProgressBar>(className: "BlueTeamProgressBar");
    }
}
