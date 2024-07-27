using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Football.Views;
using Core.Data;
using Football.Data;
using Core.Config;

namespace Football
{
    public class FieldReferenceHolder : MonoBehaviour
    {
        [SerializeField]
        public MovementView MovementView;

        public static Transform SelectedRedPlayerMark;
        public static Transform SelectedBluePlayerMark;

        [SerializeField]
        Transform _selectedRedPlayerMark;

        [SerializeField]
        Transform _selectedBluePlayerMark;

        [SerializeField]
        float _speed;

        [SerializeField]
        List<PlayerConfig> _blueTeamPlayers;

        [SerializeField]
        List<PlayerConfig> _redTeamPlayers;

        [SerializeField]
        GameObject _ball;

        [SerializeField]
        UIDocument _document;

        [SerializeField]
        List<Transform> _leftSpawnPoints;

        [SerializeField]
        List<Transform> _rightSpawnPoints;

        [SerializeField]
        GameObject _fovObject;

        [SerializeField]
        GameObject _fieldObject;

        void Awake()
        {
            SelectedRedPlayerMark = _selectedRedPlayerMark;
            SelectedBluePlayerMark = _selectedBluePlayerMark;

            //Movement Data
            MovementData.RedTeamPlayers = _redTeamPlayers;
            MovementData.BlueTeamPlayers = _blueTeamPlayers;
            MovementData.Ball = _ball;
            MovementData.BasicSpeed = _speed;
            MovementData.FovObject = _fovObject;
            MovementData.Input = new InputActions();
            MovementData.Input.Enable();

            //MatchData
            MatchData.Time = 0;
            MatchData.UItime = _document.rootVisualElement.Q<Label>(className: "time");
            MatchData.UIScore = _document.rootVisualElement.Q<Label>(className: "score");
            MatchData.RedPlayerName = _document.rootVisualElement.Q<Label>(className: "redplayername");
            MatchData.BluePlayerName = _document.rootVisualElement.Q<Label>(className: "blueplayername");
            MatchData.RedTeamBar = _document.rootVisualElement.Q<ProgressBar>(name: "RedTeamProgressBar");
            MatchData.BlueTeamBar = _document.rootVisualElement.Q<ProgressBar>(name: "BlueTeamProgressBar");
            MatchData.LeftSpawnPoints = _leftSpawnPoints;
            MatchData.RightSpawnPoints = _rightSpawnPoints;
            MatchData.FieldObject = _fieldObject;
        }
    }
}
