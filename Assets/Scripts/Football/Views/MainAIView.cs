using UnityEngine;
using Football.Controllers;
using Football.Data;
using Core.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Football.Views
{
    internal class MainAIView : MonoBehaviour
    {
        public PlayerData AIPlayer { get => MovementData.BlueSelectedPlayer.GetComponent<PlayerData>(); }
        public PlayerData EnemyPlayer { get => MovementData.RedSelectedPlayer.GetComponent<PlayerData>(); }

        bool _isPathClear;
        bool _canShoot = true;

        delegate void KickBall(float power, Vector3 direction, PlayerData data);
        event KickBall kickBall;

        void Start() => kickBall += MovementController.BallAddForce;

        void Update()
        {
            if (MatchData.BlueTeamHasBall && !MatchData.localCoop)
            {
                Main();
                MoveAI();
            }
        }

        void Main()
        {
            if (MatchData.RedTeamHasBall)
                return;

            foreach (var enemyData in MovementData.AllPlayers.Where(d => d.playerTeam != AIPlayer.playerTeam))
                if (Vector3.Distance(AIPlayer.PlayerPosition, enemyData.PlayerPosition) < 0.9f)
                {
                    _isPathClear = false;
                    if (FindPlayer(out var player))
                        Pass(player);
                    else
                        AIController.Dirbble(AIPlayer.Torso.GetComponent<Rigidbody>());
                }
                else
                    _isPathClear = true;
        }

        public void MoveAI()
        {
            if (_isPathClear)
            {
                var distance = Vector3.Distance(AIPlayer.Target, AIPlayer.PlayerPosition);
                AIPlayer.Target = new Vector3(MatchData.RedGoal.transform.position.x, 0, MatchData.RedGoal.transform.position.z);
                if (distance > 5)
                    AIController.MovePlayers(AIPlayer.Target, AIPlayer.PlayerPosition, AIPlayer);
                else
                    return;

                if (distance < 10 && _canShoot && MatchData.BlueTeamHasBall)
                {
                    kickBall?.Invoke(15, AIPlayer.Torso.transform.forward, AIPlayer);
                    KickReset(200);
                }

            }
        }

        bool FindPlayer(out PlayerData player)
        {
            if (!MovementData.PlayerHasBall)
            {
                player = null;
                return false;
            }

            var SelectedPlayer = MovementData.BlueSelectedPlayer;

            Transform selectedTransform = SelectedPlayer.transform;

            var players = MovementController.FieldOfView(MovementData.BlueFovObject, selectedTransform);

            if (players.Count <= 0)
            {
                player = null;
                return false;
            }

            player = MovementController.FindClosestPlayer(players, AIPlayer.Torso.transform, out var distance);
            return true;
        }

        bool Pass(PlayerData player)
        {
            Rigidbody playerRb = player.Torso.GetComponent<Rigidbody>();

            if (MovementController.InterceptionDirection(player.PlayerPosition, AIPlayer.PlayerPosition, playerRb.velocity, 15, out var Position, out var direction))
            {
                var vector = new Vector3(Position.x - AIPlayer.PlayerPosition.x, 0, Position.z - AIPlayer.PlayerPosition.z).normalized;
                kickBall?.Invoke(15, vector, player);
            }
            return true;
        }

        async void KickReset(int time)
        {
            _canShoot = false;
            await Task.Delay(time);
            _canShoot = true;
        }
    }
}
