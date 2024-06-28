using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Football.Views;
using Core.Data;
using Football.Data;
using Core.Config;
using Football.Controllers;

namespace Football
{
    public class FieldReferenceHolder : MonoBehaviour
    {
        [SerializeField]
        public MovementView MovementView;

        [SerializeField]
        List<GameObject> _testPlayers;

        [SerializeField]
        List<PlayerConfig> _blueTeamPlayers;

        [SerializeField]
        List<PlayerConfig> _redTeamPlayers;

        [SerializeField]
        GameObject _selectedPlayer;

        [SerializeField]
        GameObject _ball;

        [SerializeField]
        float _speed;

        [SerializeField]
        UIDocument _document;

        [SerializeField]
        List<Transform> _leftSpawnPoints;

        [SerializeField]
        List<Transform> _rightSpawnPoints;

        [SerializeField]
        GameObject _fovObject;

        void OnEnable()
        {
            //Movement Data
            MovementData.SelectedPlayer = _selectedPlayer;
            MovementData.RedTeamPlayers = _redTeamPlayers;
            MovementData.Ball = _ball;
            MovementData.BasicSpeed = _speed;
            MovementData.BlueTeamPlayers = _blueTeamPlayers;
            MovementData.TestPlayers = _testPlayers;
            MovementData.FovObject = _fovObject;

            //MatchData
            MatchData.Time = 0;
            MatchData.UItime = _document.rootVisualElement.Q<Label>(className: "time");
            MatchData.UIScore = _document.rootVisualElement.Q<Label>(className: "score");
            MatchData.RedTeamBar = _document.rootVisualElement.Q<ProgressBar>(name: "RedTeamProgressBar");
            MatchData.BlueTeamBar = _document.rootVisualElement.Q<ProgressBar>(className: "BlueTeamProgressBar");
            MatchData.LeftSpawnPoints = _leftSpawnPoints;
            MatchData.RightSpawnPoints = _rightSpawnPoints;

            SpawnController.SpawnPlayers();
        }
    }
}
