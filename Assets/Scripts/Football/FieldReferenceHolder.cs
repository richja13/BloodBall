using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Football.Views;
using Football.Data;
using Core.Config;
using Cinemachine;
using static Football.Data.MovementData;
using static Core.Data.MatchData;

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
            RedTeamPlayers = _redTeamPlayers;
            BlueTeamPlayers = _blueTeamPlayers;
            Ball = _ball;
            BasicSpeed = _speed;
            RedFovObject = _redFovObject;
            BlueFovObject = _blueFovObject;
            MovementData.Input = new InputActions();
            MovementData.Input.Enable();

            //MatchData
            Timer = 0;
            UItime = _document.rootVisualElement.Q<Label>(className: "time");
            UIScore = _document.rootVisualElement.Q<Label>(className: "score");
            RedPlayerName = _document.rootVisualElement.Q<Label>(className: "redplayername");
            BluePlayerName = _document.rootVisualElement.Q<Label>(className: "blueplayername");
            RedTeamBar = _document.rootVisualElement.Q<ProgressBar>(name: "RedTeamProgressBar");
            BlueTeamBar = _document.rootVisualElement.Q<ProgressBar>(name: "BlueTeamProgressBar");
            LeftSpawnPoints = _leftSpawnPoints;
            RightSpawnPoints = _rightSpawnPoints;
            FieldObject = _fieldObject;
            BlueGoal = _blueGoal;
            RedGoal = _redGoal;
            RedIndicator = _redIndicator;
            BlueIndicator = _blueIndicator;
            MainCamera = _cam;
        }
    }
}
