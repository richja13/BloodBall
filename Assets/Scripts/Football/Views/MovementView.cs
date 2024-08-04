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
        static bool _shootR = false;
        static bool _shootB = false;
        bool _passR = false;
        bool _passB = false;
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

            InputMap.Shoot.canceled += (context) =>
            {
                var team = (context.control.device is Mouse) ? Team.Red : Team.Blue;
                if (team == Team.Red && MatchData.RedTeamHasBall)
                    _shootR = true;
                else if(team == Team.Red && !MatchData.RedTeamHasBall)
                    MovementController.BallTackle(MovementData.RedSelectedPlayer.transform);

                if (team == Team.Blue && MatchData.BlueTeamHasBall)
                    _shootB = true;
                else if(team == Team.Blue && !MatchData.BlueTeamHasBall)
                    MovementController.BallTackle(MovementData.BlueSelectedPlayer.transform);
            };

            InputMap.Pass.canceled += (context) =>
            {
                var team = (context.control.device is Mouse) ? Team.Red : Team.Blue;
                if (team == Team.Red && MatchData.RedTeamHasBall)
                    _passR = true;
                if (team == Team.Blue && MatchData.BlueTeamHasBall)
                    _passB = true;
            };

            InputMap.Change.canceled += (context) =>
            {
                var team = (context.control.device is Keyboard) ? Team.Red : Team.Blue;
                ChangePlayer(team);
            };

            kickBall += MovementController.BallAddForce;
        }
        void Update()
        {
            SelectedPlayerIcon();

            MatchData.RedPlayerName.text = MovementData.RedSelectedPlayer.name;
            MatchData.BluePlayerName.text = MovementData.BlueSelectedPlayer.name;

            var InputMap = MovementData.Input.GamePlay;

            if(InputMap.Shoot.IsPressed())
                LoadKickForce();

            _movementVectorRed = InputMap.RedMovement.ReadValue<Vector2>();
            _movementVectorBlue = InputMap.BlueMovement.ReadValue<Vector2>();

            MovementData.RedSelectedPlayer.transform.position += MovementController.Movement(_movementVectorRed.x, _movementVectorRed.y, MovementData.BasicSpeed);
            MovementData.BlueSelectedPlayer.transform.position += MovementController.Movement(_movementVectorBlue.x, _movementVectorBlue.y, MovementData.BasicSpeed);

            MovementData.RedSelectedPlayer.transform.rotation = MovementController.Rotation(MovementData.RedSelectedPlayer.transform, _movementVectorRed);
            MovementData.BlueSelectedPlayer.transform.rotation = MovementController.Rotation(MovementData.BlueSelectedPlayer.transform, _movementVectorBlue);

            if (MovementData.PlayerHasBall)
                MovementData.Ball.transform.localPosition = new Vector3(0, -0.5f, 0.85f);

            MovementController.GetBall();
        }

        void FixedUpdate()
        {
            if (_shootB)
                Shoot(Team.Blue);

            if (_shootR)
                Shoot(Team.Red);

            if (_passR)
                Pass();

            if (_passB)
                Pass();
        }

        void LoadKickForce()
        {
            if (!MovementData.PlayerHasBall)
                return;

            _kickPower += (_kickPower < MAX_KICK_POWER) ? Time.deltaTime * 3 : 0;

            if(MatchData.RedTeamHasBall)
                CoreViewModel.LoadPowerBar(MatchData.RedTeamBar, MAX_KICK_POWER, _kickPower);

            if(MatchData.BlueTeamHasBall)
                CoreViewModel.LoadPowerBar(MatchData.BlueTeamBar, MAX_KICK_POWER, _kickPower);
        }

        void Shoot(Team team)
        {
            _shootR = false;
            _shootB = false;

            if (MatchData.RedTeamHasBall)
                CoreViewModel.LoadPowerBar(MatchData.RedTeamBar, MAX_KICK_POWER, 0);
            
            if(MatchData.BlueTeamHasBall)
                CoreViewModel.LoadPowerBar(MatchData.BlueTeamBar, MAX_KICK_POWER, 0);

            var SelectedPlayer = (MatchData.RedTeamHasBall) ? MovementData.RedSelectedPlayer : (MatchData.BlueTeamHasBall) ? MovementData.BlueSelectedPlayer : null;

            if (SelectedPlayer?.GetComponent<PlayerData>().playerTeam == team)
            {
                kickBall?.Invoke((6 * _kickPower) + 30, new Vector3(SelectedPlayer.transform.forward.x, 0.2f,SelectedPlayer.transform.forward.z));

                _kickPower = 0;
                MatchData.RedTeamHasBall = false;
                MatchData.BlueTeamHasBall = false;
            }
        }

        async void Pass()
        {
            _passR = false;
            _passB = false;

            if (!MovementData.PlayerHasBall)
                return;

            var SelectedPlayer = (MatchData.RedTeamHasBall) ? MovementData.RedSelectedPlayer : MovementData.BlueSelectedPlayer;

            var players = MovementController.FieldOfView(MovementData.FovObject, SelectedPlayer.transform);

            if (players is null || players.Count <= 0)
                return;

            var closestPlayer = MovementController.FindClosestPlayer(players, SelectedPlayer.transform, out var distance);
            SelectedPlayer.transform.LookAt(closestPlayer);
            MovementData.Ball.transform.LookAt(SelectedPlayer.transform);

            Vector3 PlayerTarget = closestPlayer.GetComponent<PlayerData>().Target;
            Vector3 direction = PlayerTarget - closestPlayer.position;
            float targetSpeed = 10 * Time.deltaTime;
            Vector3 VelocityVector = (direction.normalized * targetSpeed);
            Vector3 FutureVector = closestPlayer.position + VelocityVector * 3;
            var vector = new Vector3(FutureVector.x - SelectedPlayer.transform.position.x, 0, FutureVector.z - SelectedPlayer.transform.position.z).normalized;

            kickBall?.Invoke(32, vector);
            MatchData.RedTeamHasBall = false;
            MatchData.BlueTeamHasBall = false;

            await Task.Delay((int)distance * 80);

            SelectedPlayer = closestPlayer.gameObject;
        }

        void ChangePlayer(Team team)
        {
            Transform closestPlayer;
            if (team == Team.Red)
            {
                closestPlayer = MovementController.FindClosestPlayer(MovementData.RedTeam, MovementData.Ball.transform, out var distance);
                MovementData.RedSelectedPlayer = closestPlayer.gameObject;
            }
            else
            {
                closestPlayer = MovementController.FindClosestPlayer(MovementData.BlueTeam, MovementData.Ball.transform, out var distance);
                MovementData.BlueSelectedPlayer = closestPlayer.gameObject;
            }
        }

        void SelectedPlayerIcon()
        {
            FieldReferenceHolder.SelectedRedPlayerMark.position = MovementData.RedSelectedPlayer.transform.position;
            FieldReferenceHolder.SelectedBluePlayerMark.position = MovementData.BlueSelectedPlayer.transform.position;
        }
    }
}
