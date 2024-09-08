using System.Threading.Tasks;
using UnityEngine;
using Football.Controllers;
using Football.Data;
using Core.Data;
using Core;
using Core.Enums;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UIElements;

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
                else if (team == Team.Red && !MatchData.RedTeamHasBall)
                    MovementData.RedSelectedPlayer.GetComponent<PlayerData>().InvokeAttack();

                if (team == Team.Blue && MatchData.BlueTeamHasBall)
                        _shootB = true;
                else if (team == Team.Blue && !MatchData.BlueTeamHasBall)
                    MovementData.BlueSelectedPlayer.GetComponent<PlayerData>().InvokeAttack();
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

            MovementData.RedSelectedPlayer.GetComponent<PlayerData>().Movement = new Vector3(_movementVectorRed.x, 0,_movementVectorRed.y);
            MovementData.BlueSelectedPlayer.GetComponent<PlayerData>().Movement = new Vector3(_movementVectorBlue.x, 0, _movementVectorBlue.y);

            if (MovementData.PlayerHasBall)
                MovementData.Ball.transform.localPosition = new Vector3(0, -.3f, 0.85f);

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
            var data = SelectedPlayer.GetComponent<PlayerData>();

            if (data.playerTeam == team)
            {
                kickBall?.Invoke(_kickPower * 6.5f, new Vector3(data.PlayerRotation.normalized.x, 0.2f,data.PlayerRotation.normalized.z));

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

            PlayerData data = SelectedPlayer.GetComponent<PlayerData>();

            var closestPlayer = MovementController.FindClosestPlayer(players, data.Torso.transform, out var distance);
            Rigidbody playerRb = closestPlayer.Torso.GetComponent<Rigidbody>();

            if (MovementController.InterceptionDirection(closestPlayer.PlayerPosition, data.PlayerPosition, playerRb.velocity, 15, out var Position, out var direction))
            {
                var vector = new Vector3(Position.x - data.PlayerPosition.x, 0, Position.z - data.PlayerPosition.z).normalized;
                kickBall?.Invoke(15, vector);
            }
            else
            {
                Vector3 PlayerTarget = closestPlayer.Target;
                Vector3 dir = PlayerTarget - closestPlayer.PlayerPosition;
                float targetSpeed = 10 * Time.deltaTime;
                Vector3 VelocityVector = dir.normalized * targetSpeed;
                Vector3 FutureVector = closestPlayer.PlayerPosition + VelocityVector * 3;
                var vector = new Vector3(FutureVector.x - data.PlayerPosition.x, 0, FutureVector.z - data.PlayerPosition.z).normalized;
                kickBall?.Invoke(13, vector);
            }

            MatchData.RedTeamHasBall = false;
            MatchData.BlueTeamHasBall = false;

            await Task.Delay((int)distance * 80);

            SelectedPlayer = closestPlayer.gameObject;
        }

        void ChangePlayer(Team team)
        {
            PlayerData closestPlayer;
            if (team == Team.Red)
            {
                PlayerData selectedPlayer = MovementData.RedSelectedPlayer.GetComponent<PlayerData>();
                closestPlayer = MovementController.FindClosestPlayer(MovementData.AllPlayers.Where(data => data.playerTeam == Team.Red && data != selectedPlayer).ToList(), MovementData.Ball.transform, out var distance);
                Debug.Log("Closest Player is: " + closestPlayer.name);
                MovementData.RedSelectedPlayer = closestPlayer.gameObject;
            }
            else
            {
                PlayerData selectedPlayer = MovementData.BlueSelectedPlayer.GetComponent<PlayerData>();
                closestPlayer = MovementController.FindClosestPlayer(MovementData.AllPlayers.Where(data => data.playerTeam == Team.Blue && data != selectedPlayer).ToList(), MovementData.Ball.transform, out var distance);
                MovementData.BlueSelectedPlayer = closestPlayer.gameObject;
            }
        }

        void SelectedPlayerIcon()
        {
            FieldReferenceHolder.SelectedRedPlayerMark.position = MovementData.RedSelectedPlayer.GetComponent<PlayerData>().PlayerPosition;
            FieldReferenceHolder.SelectedBluePlayerMark.position = MovementData.BlueSelectedPlayer.GetComponent<PlayerData>().PlayerPosition;
        }
    }
}
