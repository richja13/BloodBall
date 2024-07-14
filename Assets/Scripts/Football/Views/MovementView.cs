using System.Threading.Tasks;
using UnityEngine;
using Football.Controllers;
using Football.Data;
using Core.Data;
using Core;
using System.Collections.Generic;
using Core.Enums;
using UnityEngine.InputSystem;

namespace Football.Views
{
    public class MovementView : MonoBehaviour
    {
        [SerializeField]
        PlayerInputManager manager;

        bool _shoot = false;
        bool _pass = false;
        float _kickPower = 2;
        Vector2 _movementVectorRed;
        Vector2 _movementVectorBlue;
        const float MAX_KICK_POWER = 4;

        delegate void KickBall(float power, Vector3 direction);
        event KickBall kickBall;
    
        void OnEnable() => SpawnController.SpawnPlayers();

        void Start()
        {
            var InputMap = MovementData.Input.GamePlay;
            InputMap.Shoot.canceled += _ => _shoot = true;
            InputMap.Pass.canceled += _ => _pass = true;
            InputMap.Change.canceled += (context) =>
            {
                var team = (context.control.device is Keyboard) ? Team.Red : Team.Blue;
                ChangePlayer(team);
            };

            kickBall += MovementController.BallAddForce;
        }
        void Update()
        {
            MatchData.RedPlayerName.text = MovementData.RedSelectedPlayer.name;
            MatchData.BluePlayerName.text = MovementData.BlueSelectedPlayer.name;

            var InputMap = MovementData.Input.GamePlay;

            if(InputMap.Shoot.IsPressed())
                LoadKickForce();

            _movementVectorRed = InputMap.RedMovement.ReadValue<Vector2>();
            _movementVectorBlue = InputMap.BlueMovement.ReadValue<Vector2>();

            MovementData.RedSelectedPlayer.transform.position += MovementController.Movement(_movementVectorRed.x, _movementVectorRed.y, MovementData.BasicSpeed);
            MovementData.BlueSelectedPlayer.transform.position += MovementController.Movement(_movementVectorBlue.x, _movementVectorBlue.y, MovementData.BasicSpeed);

            MovementData.RedSelectedPlayer.transform.rotation = (_movementVectorRed.x != 0 || _movementVectorRed.y != 0) ? 
                Quaternion.Slerp(MovementData.RedSelectedPlayer.transform.rotation, Quaternion.LookRotation(new Vector3(_movementVectorRed.x + 0.1f, 0, _movementVectorRed.y + 0.1f)), Time.deltaTime * 5) :
                MovementData.RedSelectedPlayer.transform.rotation;

            MovementData.BlueSelectedPlayer.transform.rotation = (_movementVectorBlue.x != 0 || _movementVectorBlue.y != 0) ?
                Quaternion.Slerp(MovementData.BlueSelectedPlayer.transform.rotation, Quaternion.LookRotation(new Vector3(_movementVectorBlue.x + 0.1f, 0, _movementVectorBlue.y + 0.1f)), Time.deltaTime * 5) :
                MovementData.BlueSelectedPlayer.transform.rotation;

            if (MovementData.PlayerHasBall)
                MovementData.Ball.transform.localPosition = new Vector3(0, -0.5f, 0.85f);

            MovementController.GetBall();
        }

        void FixedUpdate()
        {
            if (_shoot)
                Shoot();

            if (_pass)
                Pass(Team.Red);
        }

        void LoadKickForce()
        {
            if (!MovementData.PlayerHasBall)
                return;

            _kickPower += (_kickPower < MAX_KICK_POWER) ? Time.deltaTime * 3 : 0;
            CoreViewModel.LoadPowerBar(MatchData.RedTeamBar, MAX_KICK_POWER, _kickPower);
        }

        void Shoot()
        {
            _shoot = false;

            CoreViewModel.LoadPowerBar(MatchData.RedTeamBar, MAX_KICK_POWER, 0);

            if (!MovementData.PlayerHasBall)
                return;

            Debug.Log($"Kick power: {_kickPower}");

            kickBall?.Invoke((10 * _kickPower) + 15, MovementData.RedSelectedPlayer.transform.forward);
            _kickPower = 0;
            MatchData.RedTeamHasBall = false;
        }

        async void Pass(Team team)
        {
            _pass = false;

            if (!MovementData.PlayerHasBall)
                return;

            List<GameObject> playerModels = new();
            MovementData.RedTeamPlayers.ForEach(playerModel => playerModels.Add(playerModel.PlayerModel));

            var SelectedPlayer = (team == Team.Red) ? MovementData.RedSelectedPlayer : MovementData.BlueSelectedPlayer;

            var players = MovementController.FieldOfView(MovementData.FovObject, SelectedPlayer.transform);

            if (players is null || players.Count <= 0)
                return;

            var closestPlayer = MovementController.FindClosestPlayer(players, SelectedPlayer.transform, out var distance);
            SelectedPlayer.transform.LookAt(closestPlayer);
            MovementData.Ball.transform.LookAt(SelectedPlayer.transform);
            closestPlayer.LookAt(SelectedPlayer.transform);
            var vector = new Vector3(closestPlayer.position.x - SelectedPlayer.transform.position.x, 0, closestPlayer.position.z - SelectedPlayer.transform.position.z) / 13;
            kickBall?.Invoke(15, vector);
            MatchData.RedTeamHasBall = false;

            await Task.Delay((int)distance * 80);

            SelectedPlayer = closestPlayer.gameObject;
        }

        void ChangePlayer(Team team)
        {
            List<GameObject> playerModels = new();
            MovementData.RedTeamPlayers.ForEach(playerModel => { playerModels.Add(playerModel.PlayerModel); });

            var SelectedPlayer = (team == Team.Red) ? MovementData.RedSelectedPlayer : MovementData.BlueSelectedPlayer;

            var closestPlayer = MovementController.FindClosestPlayer(MovementData.RedTeam, SelectedPlayer.transform, out var distance);
            SelectedPlayer = closestPlayer.gameObject;
        }
    }
}
