using System.Threading.Tasks;
using UnityEngine;
using Football.Controllers;
using Football.Data;
using Core.Data;
using Core;
using Core.Enums;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;

namespace Football.Views
{
    public class MovementView : MonoBehaviour
    {
        public static MovementView Instance;

        static bool _shootR = false;
        static bool _shootB = false;
        bool _passR = false;
        bool _passB = false;
        float _kickPower = 2;
        Vector2 _movementVectorRed;
        Vector2 _movementVectorBlue;
        const float MAX_KICK_POWER = 4;

        delegate void KickBall(float power, Vector3 direction, PlayerData player);
        event KickBall kickBall;

        void Awake() => Instance = this;

        void Start()
        {
            InitializeControllers(MatchData.LocalCoop);

            Debug.Log($"Local Coop: {MatchData.LocalCoop}");

            kickBall += MovementController.BallAddForce;
        }

        void OnEnable() => SpawnController.SpawnPlayers();

        void InitializeControllers(bool LocalCoop)
        {
            var InputMap = MovementData.Input.GamePlay;

            InputMap.Shoot.canceled += (context) =>
            {
                var team = (context.control.device is Mouse) ? Team.Red : Team.Blue;
                if (team == Team.Red && MatchData.RedTeamHasBall)
                    _shootR = true;
                else if (team == Team.Red && !MatchData.RedTeamHasBall)
                    MovementData.RedSelectedPlayer.GetComponent<PlayerData>().InvokeAttack();

                if (LocalCoop)
                {
                    if (team == Team.Blue && MatchData.BlueTeamHasBall)
                        _shootB = true;
                    else if (team == Team.Blue && !MatchData.BlueTeamHasBall)
                        MovementData.BlueSelectedPlayer.GetComponent<PlayerData>().InvokeAttack();
                }
            };

            InputMap.Pass.canceled += (context) =>
            {
                var team = (context.control.device is Mouse) ? Team.Red : Team.Blue;
                if (team == Team.Red && MatchData.RedTeamHasBall)
                    _passR = true;

                if (LocalCoop)
                    if (team == Team.Blue && MatchData.BlueTeamHasBall)
                    _passB = true;
            };

            InputMap.Change.canceled += (context) =>
            {
                Team team;
                if (LocalCoop)
                    team = (context.control.device is Keyboard) ? Team.Red : Team.Blue;
                else
                    team = Team.Red;

                ChangePlayer(team);
            };
        }

        internal void CustomUpdate()
        {

            SelectedPlayerIcon();

            MatchData.RedPlayerName.text = MovementData.RedSelectedPlayer.name;
            MatchData.BluePlayerName.text = MovementData.BlueSelectedPlayer.name;

            var InputMap = MovementData.Input.GamePlay;

            if(InputMap.Shoot.IsPressed())
                LoadKickForce();

            _movementVectorRed = InputMap.RedMovement.ReadValue<Vector2>();
            PlayerData data = MovementData.RedSelectedPlayer.GetComponent<PlayerData>();
            Vector3 vector = (MovementData.Ball.transform.position - data.PlayerPosition).normalized;

            if (MatchData.RedTeamHasBall)
            {
                if (_movementVectorRed != Vector2.zero)
                    data.Movement = new Vector3(vector.x, 0, vector.z);
                else
                    data.Movement = Vector3.zero;

                data.Target = new Vector3(_movementVectorRed.x, 0, _movementVectorRed.y);
            }
            else
            {
                data.Movement = new Vector3(_movementVectorRed.x, 0, _movementVectorRed.y);
            }

       /*     //Sprint
           if(data.Movement != Vector3.zero)
                data.Torso.GetComponent<Rigidbody>().AddForce(new Vector3(data.Torso.transform.forward.x, 0, data.Torso.transform.forward.z)* 5, ForceMode.Impulse);*/

            if (MatchData.LocalCoop)
            {
                _movementVectorBlue = InputMap.BlueMovement.ReadValue<Vector2>();
                data = MovementData.BlueSelectedPlayer.GetComponent<PlayerData>();
                vector = (MovementData.Ball.transform.position - data.PlayerPosition).normalized;

                if (MatchData.BlueTeamHasBall)
                {
                    if (_movementVectorBlue != Vector2.zero)
                        data.Movement = new Vector3(vector.x, 0, vector.z);
                    else
                        data.Movement = Vector3.zero;

                    data.Target = new Vector3(_movementVectorBlue.x, 0, _movementVectorBlue.y);
                }
                else
                {
                    data.Movement = new Vector3(_movementVectorBlue.x, 0, _movementVectorBlue.y);
                }
            }

            /*      if (MovementData.Ball.transform.parent != null)
                  {
                      MovementData.Ball.transform.localPosition = new Vector3(0, -.3f, 0.85f);
                  }
                  else
                      MovementData.PlayerHasBall = false;*/

#if UNITY_EDITOR
            ShowAvailablePlayers();
#endif
        }

        internal void CustomFixedUpdate()
        {
            if (_shootB)
                Shoot(Team.Blue);

            if (_shootR)
                Shoot(Team.Red);

            if (_passR)
                Pass();

            if (_passB)
                Pass();

            MovementController.GetBall();
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

            CoreViewModel.LoadPowerBar(MatchData.RedTeamBar, MAX_KICK_POWER, 0);
            CoreViewModel.LoadPowerBar(MatchData.BlueTeamBar, MAX_KICK_POWER, 0);

            var SelectedPlayer = (MatchData.RedTeamHasBall) ? MovementData.RedSelectedPlayer : (MatchData.BlueTeamHasBall) ? MovementData.BlueSelectedPlayer : null;
            var data = SelectedPlayer.GetComponent<PlayerData>();

            if (data.playerTeam == team)
            {
                //kickBall?.Invoke(_kickPower * 6.5f, new Vector3(data.PlayerRotation.normalized.x, 0.2f,data.PlayerRotation.normalized.z), data);
                kickBall?.Invoke(_kickPower * 6.5f, new Vector3(data.Movement.x, 0.2f,data.Movement.z), data);

                _kickPower = 0;
            }
        }

        async void Pass()
        {
            _passR = false;
            _passB = false;

            if (!MovementData.PlayerHasBall)
                return;

            var SelectedPlayer = (MatchData.RedTeamHasBall) ? MovementData.RedSelectedPlayer : MovementData.BlueSelectedPlayer;

            Transform selectedTransform = SelectedPlayer.transform;

            var players = MovementController.FieldOfView((MatchData.RedTeamHasBall) ? MovementData.RedFovObject : MovementData.BlueFovObject, selectedTransform);

            if (players.Count <= 0)
                return;

            PlayerData data = SelectedPlayer.GetComponent<PlayerData>();

            var closestPlayer = MovementController.FindClosestPlayer(players, data.Torso.transform, out var distance);
            Rigidbody playerRb = closestPlayer.Torso.GetComponent<Rigidbody>();

            if (MovementController.InterceptionDirection(closestPlayer.PlayerPosition, data.PlayerPosition, playerRb.velocity, 15, out var Position, out var direction))
            {
                Vector3 vector = new Vector3(Position.x - data.PlayerPosition.x, 0, Position.z - data.PlayerPosition.z).normalized;
                var power = (distance < 7) ? 15 * distance/7 : 15;
                kickBall?.Invoke(power, vector, data);
                Vector3 movementVector = new Vector3(MovementData.Ball.transform.position.x - closestPlayer.PlayerPosition.x, 0, MovementData.Ball.transform.position.z - closestPlayer.PlayerPosition.z).normalized;
                closestPlayer.Movement = movementVector;

                if(distance > 5)
                    closestPlayer.Torso.GetComponent<Rigidbody>().AddForce(300 * movementVector, ForceMode.Impulse);
            }

            MatchData.RedTeamHasBall = false;
            MatchData.BlueTeamHasBall = false;
         
            await Task.Delay((int)distance * 80);

            if(data.playerTeam == Team.Red)
             MovementData.RedSelectedPlayer = closestPlayer.gameObject;
            else
                MovementData.BlueSelectedPlayer = closestPlayer.gameObject;

        }

        void ChangePlayer(Team team)
        {
            PlayerData closestPlayer;
            if (team == Team.Red)
            {
                PlayerData selectedPlayer = MovementData.RedSelectedPlayer.GetComponent<PlayerData>();
                closestPlayer = MovementController.FindClosestPlayer(MovementData.AllPlayers.Where(data => data.playerTeam == Team.Red && data != selectedPlayer).ToList(), MovementData.Ball.transform, !MatchData.RedTeamHasBall,out var distance);
                MovementData.RedSelectedPlayer = closestPlayer.gameObject;
            }
            else
            {
                PlayerData selectedPlayer = MovementData.BlueSelectedPlayer.GetComponent<PlayerData>();
                closestPlayer = MovementController.FindClosestPlayer(MovementData.AllPlayers.Where(data => data.playerTeam == Team.Blue && data != selectedPlayer).ToList(), MovementData.Ball.transform, MatchData.BlueTeamHasBall,out var distance);
                MovementData.BlueSelectedPlayer = closestPlayer.gameObject;
            }
        }

        void SelectedPlayerIcon()
        {
            FieldReferenceHolder.SelectedRedPlayerMark.position = MovementData.RedSelectedPlayer.GetComponent<PlayerData>().PlayerPosition;
            FieldReferenceHolder.SelectedBluePlayerMark.position = MovementData.BlueSelectedPlayer.GetComponent<PlayerData>().PlayerPosition;
        }

        List<PlayerData> _players = new();

        void ShowAvailablePlayers()
        {
            if (!MovementData.PlayerHasBall)
                return;

            var SelectedPlayer = (MatchData.RedTeamHasBall) ? MovementData.RedSelectedPlayer : MovementData.BlueSelectedPlayer;

            Transform selectedTransform = SelectedPlayer.transform; 

            _players = MovementController.FieldOfView(MatchData.RedTeamHasBall ? MovementData.RedFovObject : MovementData.BlueFovObject, selectedTransform);
        }

        private void OnDrawGizmos()
        {
            foreach(PlayerData player in _players)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(player.PlayerPosition, new(1, 1, 1) );
            }
        }
    }
}
