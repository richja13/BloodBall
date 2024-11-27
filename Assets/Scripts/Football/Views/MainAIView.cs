using UnityEngine;
using Football.Controllers;
using Football.Data;
using Core.Data;
using System.Linq;
using System.Threading.Tasks;
using static Football.Controllers.AIController;

namespace Football.Views
{
    internal class MainAIView : MonoBehaviour
    {
        internal static MainAIView Instance;

        public PlayerData AIPlayer { get => MovementData.BlueSelectedPlayer; }
        public PlayerData EnemyPlayer { get => MovementData.RedSelectedPlayer; }

        bool _isPathClear;
        bool _canShoot = true;

        delegate void KickBall(float power, Vector3 direction, PlayerData data);
        event KickBall kickBall;

        void Awake() => Instance = this;

        void Start() => kickBall += MovementController.BallAddForce;

        internal void CustomUpdate()
        {
            if (MatchData.BlueTeamHasBall && !MatchData.LocalCoop)
            {
                Main();
                MoveAI();
            }
        }

        void Main()
        {
            if (!MatchData.MatchStarted)
                return;

            if (MatchData.RedTeamHasBall)
                return;

            if (!MatchData.BallOut)
                foreach (var enemyData in MovementData.AllPlayers.Where(d => d.playerTeam != AIPlayer.playerTeam))
                    if (Vector3.Distance(AIPlayer.PlayerPosition, enemyData.PlayerPosition) < 0.9f)
                    {
                        _isPathClear = false;
                        if (FindPlayer(out var player))
                            Pass(player);
                        else
                        {
                            var direction = (enemyData.PlayerPosition.x > AIPlayer.PlayerPosition.x) ? 1 : -1;
                            Dirbble(AIPlayer, AIPlayer.Torso.GetComponent<Rigidbody>(), direction, MovementData.Ball.GetComponent<Rigidbody>());
                        }
                    }
                    else
                        _isPathClear = true;
            else
                BallOut();
        }

        async void BallOut()
        {
            await Task.Delay(2000);
            if (FindPlayer(out var player) && MatchData.BallOut)
                Pass(player);
            else if(MatchData.BallOut)
                MovementData.Ball.GetComponent<Rigidbody>().AddForce(7 * Time.fixedDeltaTime * AIPlayer.Torso.transform.forward, ForceMode.VelocityChange);

            await Task.Delay(200);
            MovementController.EnableMovement();
        }

        void MoveAI()
        {
            if (_isPathClear)
            {
                Vector3 target = new(MatchData.RedGoal.transform.position.x, 0, MatchData.RedGoal.transform.position.z);
                AIPlayer.Target = (target - MovementData.Ball.transform.position).normalized;

                float distance = Vector3.Distance(target, AIPlayer.PlayerPosition);

                if (distance > 5)
                    MovePlayers(MovementData.Ball.transform.position, AIPlayer.PlayerPosition, AIPlayer);
                else
                    AIPlayer.Movement = Vector3.zero;

                if (distance < 10 && _canShoot && MatchData.BlueTeamHasBall)
                {
                    kickBall?.Invoke(17, AIPlayer.Target, AIPlayer);
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

            player = MovementController.FindClosestPlayer(players, AIPlayer.Torso.transform, out _);
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
