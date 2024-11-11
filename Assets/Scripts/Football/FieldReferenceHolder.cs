using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Football.Views;
using Core.Data;
using Football.Data;
using Core.Config;
using Cinemachine;

namespace Football
{
    public class FieldReferenceHolder : MonoBehaviour
    {
        [SerializeField]
        public MovementView MovementView;

        public static Transform SelectedRedPlayerMark;
        public static Transform SelectedBluePlayerMark;

        public static ParticleSystem BallHitEffect;

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
        GameObject _redFovObject;

        [SerializeField]
        GameObject _blueFovObject;

        [SerializeField]
        GameObject _fieldObject;

        [SerializeField]
        ParticleSystem _ballHitEffect;

        [SerializeField]
        GameObject _blueGoal;

        [SerializeField]
        GameObject _redGoal;

        [SerializeField]
        RectTransform _redIndicator;

        [SerializeField]
        RectTransform _blueIndicator;

        [SerializeField]
        CinemachineVirtualCamera _cam;

        void Awake()
        {
            SelectedRedPlayerMark = _selectedRedPlayerMark;
            SelectedBluePlayerMark = _selectedBluePlayerMark;
            BallHitEffect = _ballHitEffect;

            //Movement Data
            MovementData.RedTeamPlayers = _redTeamPlayers;
            MovementData.BlueTeamPlayers = _blueTeamPlayers;
            MovementData.Ball = _ball;
            MovementData.BasicSpeed = _speed;
            MovementData.RedFovObject = _redFovObject;
            MovementData.BlueFovObject = _blueFovObject;
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
            MatchData.BlueGoal = _blueGoal;
            MatchData.RedGoal = _redGoal;
            MatchData.RedIndicator = _redIndicator;
            MatchData.BlueIndicator = _blueIndicator;
            MatchData.Camera = _cam;
        }
    }
}
