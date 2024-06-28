using System.Threading.Tasks;
using UnityEngine;
using Football.Controllers;
using Football.Data;
using Core.Data;
using Core;
using System.Collections.Generic;

namespace Football.Views
{
    public class MovementView : MonoBehaviour
    {
        bool _shoot = false;
        bool _pass = false;
        float _kickPower = 2;
        const float MAX_KICK_POWER = 4;

        delegate void KickBall(float power, Vector3 direction);
        event KickBall kickBall;

        void OnEnable() => SpawnController.SpawnPlayers();

        void Start() => kickBall += MovementController.BallAddForce;

        void Update()
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            MovementData.SelectedPlayer.transform.position += MovementController.Movement(x, y, MovementData.BasicSpeed);

            MovementData.SelectedPlayer.transform.rotation = ((x + y) != 0) ? 
                Quaternion.Slerp(MovementData.SelectedPlayer.transform.rotation, Quaternion.Euler(0,MovementController.RotationY(x, y), 0), Time.deltaTime * 5) :
                MovementData.SelectedPlayer.transform.rotation;

            if (MovementData.PlayerHasBall)
                MovementData.Ball.transform.localPosition = new Vector3(0, -0.5f, 0.85f);

            MovementController.GetBall();

            if (Input.GetMouseButton(0))
                LoadKickForce();

            if (Input.GetMouseButtonUp(0))
                _shoot = true;

            if (Input.GetMouseButtonDown(1))
                _pass = true;

            if (Input.GetKeyDown(KeyCode.Space))
                ChangePlayer();
        }

        void FixedUpdate()
        {
            if (_shoot)
                Shoot();

            if (_pass)
                Pass();
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

            kickBall?.Invoke(10 * _kickPower + 5, MovementData.SelectedPlayer.transform.forward);
            _kickPower = 0;
        }

        async void Pass()
        {
            _pass = false;

            if (!MovementData.PlayerHasBall)
                return;

            List<GameObject> playerModels = new();
            MovementData.RedTeamPlayers.ForEach(playerModel => playerModels.Add(playerModel.PlayerModel));

            var players = MovementController.FieldOfView(MovementData.FovObject, MovementData.SelectedPlayer.transform);

            if (players is null || players.Count <= 0)
                return;

            var closestPlayer = MovementController.FindClosestPlayer(players, MovementData.SelectedPlayer.transform, out var distance);
            MovementData.SelectedPlayer.transform.LookAt(closestPlayer);
            MovementData.Ball.transform.LookAt(MovementData.SelectedPlayer.transform);
            closestPlayer.LookAt(MovementData.SelectedPlayer.transform);
            var vector = new Vector3(closestPlayer.position.x - MovementData.SelectedPlayer.transform.position.x, 0, closestPlayer.position.z - MovementData.SelectedPlayer.transform.position.z) / 13;
            kickBall?.Invoke(15, vector);

            var a = 0;
            while (a < 40)
            {
                a++;
                var playerVector = Vector3.MoveTowards(closestPlayer.transform.position, MovementData.Ball.transform.position, 5f * Time.fixedDeltaTime);
                playerVector.y = MovementData.SelectedPlayer.transform.position.y;
                closestPlayer.transform.position = playerVector;
                await Task.Delay(10);
            }

            MovementData.SelectedPlayer = closestPlayer.gameObject;
        }

        void ChangePlayer()
        {
            List<GameObject> playerModels = new();
            MovementData.RedTeamPlayers.ForEach(playerModel => { playerModels.Add(playerModel.PlayerModel); });

            var closestPlayer = MovementController.FindClosestPlayer(MovementData.RedTeam, MovementData.SelectedPlayer.transform, out var distance);
            MovementData.SelectedPlayer = closestPlayer.gameObject;
        }
    }
}
