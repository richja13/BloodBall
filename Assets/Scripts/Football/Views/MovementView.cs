using System.Threading.Tasks;
using UnityEngine;
using Football.Controllers;
using Core;
using Football.Data;
using Core.Enums;
using System.Collections.Generic;
using Core.Signal;
using static Football.Controllers.MovementController;
using static Football.Data.MovementData;
using static Core.Data.MatchData;

namespace Football.Views
{
    public class MovementView : MonoBehaviour
    {
        public static MovementView Instance;
        float _kickPower = .3f;
        Vector2 _movementVectorRed;
        Vector2 _movementVectorBlue;
        const float MAX_KICK_POWER = 4;
        internal delegate void KickBall(float power, Vector3? direction, PlayerData player);
        internal static event KickBall kickBall;

        void Awake() => Instance = this;

        void Start()
        {
            InitializeControllers(LocalCoop, MovementData.Input);
            SpawnController.SpawnPlayers();
            kickBall += BallAddForce;

            Signals.Get<EnableMovementSignal>().AddListener(EnableMovement);
        }

        internal void CustomUpdate()
        {
            RedPlayerName.text = RedSelectedPlayer.name;
            BluePlayerName.text = BlueSelectedPlayer.name;

            SelectedPlayerIcon();

            var InputMap = MovementData.Input.GamePlay;

            if(InputMap.Shoot.IsPressed() && (!BallOut || Corner))
                LoadKickForce();

            _movementVectorRed = InputMap.RedMovement.ReadValue<Vector2>();
            PlayerData data = RedSelectedPlayer;
            Vector3 vector = (Ball.transform.position - data.PlayerPosition).normalized;

            if (RedTeamHasBall)
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

            if (LocalCoop)
            {
                _movementVectorBlue = InputMap.BlueMovement.ReadValue<Vector2>();
                data = BlueSelectedPlayer;
                vector = (Ball.transform.position - data.PlayerPosition).normalized;

                if (BlueTeamHasBall)
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

            /*     //Sprint
     if(data.Movement != Vector3.zero)
          data.Torso.GetComponent<Rigidbody>().AddForce(new Vector3(data.Torso.transform.forward.x, 0, data.Torso.transform.forward.z)* 5, ForceMode.Impulse);*/

#if UNITY_EDITOR
            ShowAvailablePlayers();
#endif
        }

        internal void CustomFixedUpdate()
        {
            if (ShootB)
                Shoot(Team.Blue);

            if (ShootR)
                Shoot(Team.Red);

            if (PassBall)
                Pass();

            GetBall();
        }

        void LoadKickForce()
        {
            if (!PlayerHasBall && !Corner)
                return;

            _kickPower += (_kickPower < MAX_KICK_POWER) ? Time.deltaTime * 3 : 0;

            if(RedTeamHasBall)
                CoreViewModel.LoadPowerBar(RedTeamBar, MAX_KICK_POWER, _kickPower);

            if(BlueTeamHasBall)
                CoreViewModel.LoadPowerBar(BlueTeamBar, MAX_KICK_POWER, _kickPower);
        }

        void Shoot(Team team)
        {
            if (BallOut && !Corner)
                return;

            ShootR = ShootB = false;

            CoreViewModel.LoadPowerBar(RedTeamBar, MAX_KICK_POWER, 0);
            CoreViewModel.LoadPowerBar(BlueTeamBar, MAX_KICK_POWER, 0);

            PlayerData data = (RedTeamHasBall) ? RedSelectedPlayer : (BlueTeamHasBall) ? BlueSelectedPlayer : null;
            Vector3? shootVector = (data?.Movement != Vector3.zero) ? new(data.Target.x, .2f, data.Target.z) : data?.Torso.transform.forward;

            if (Corner) EnableMovement();

            if (data.playerTeam == team)
            {
                kickBall?.Invoke(_kickPower * 6, shootVector, data);
                _kickPower = .3f;
            }
        }

        async void Pass()
        {
            if (!PlayerHasBall || !MovementController.Pass(out var playerRb, out var closestPlayer, out var data, out var distance))
                return;

            if (InterceptionDirection(closestPlayer.PlayerPosition, data.PlayerPosition, playerRb.velocity, 15, out var Position, out var direction))
            {
                Vector3 vector = new Vector3(Position.x - data.PlayerPosition.x, 0, Position.z - data.PlayerPosition.z).normalized;
                var power = (distance < 7) ? 15 * distance / 7 : 15;
                kickBall?.Invoke(power, vector, data);
                StartCoroutine(closestPlayer.CatchBall(Position, distance));
                closestPlayer.ExtraReach = 0.4f;
            }

            RedTeamHasBall = false;
            BlueTeamHasBall = false;

            if (BallOut) EnableMovement();

            await Task.Delay((int)distance * 80);

            if (data.playerTeam == Team.Red)
                RedSelectedPlayer = closestPlayer;
            else
                BlueSelectedPlayer = closestPlayer;

        }

        void SelectedPlayerIcon()
        {
            FieldReferenceHolder.SelectedRedPlayerMark.position = RedSelectedPlayer.PlayerPosition;
            FieldReferenceHolder.SelectedBluePlayerMark.position = BlueSelectedPlayer.PlayerPosition;
        }

        List<PlayerData> _players = new();

        void ShowAvailablePlayers()
        {
            if (!PlayerHasBall)
                return;

            var SelectedPlayer = (RedTeamHasBall) ? RedSelectedPlayer : BlueSelectedPlayer;

            Transform selectedTransform = SelectedPlayer.transform; 

            _players = FieldOfView(RedTeamHasBall ? RedFovObject : BlueFovObject, selectedTransform);
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
