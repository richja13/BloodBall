using Core;
using Core.Signal;
using Football.Controllers;
using Football.Data;
using System.Threading.Tasks;
using UnityEngine;

namespace Football.Views
{
    internal class GoalKeeperTestView : MonoBehaviour
    {
        public PlayerData PlayerData;
        public bool Jumping = true;
        [SerializeField]
        float _force = 250;
        [SerializeField]
        GameObject _ball;
        Vector3 _startingPos;

        void OnEnable()
        {
            PlayerData = GetComponent<PlayerData>();
            Jumping = true;
            Signals.Get<BallShootSignal>().AddListener(GoalKeeperLogic);
            _startingPos = new(transform.position.x, 0, transform.position.z);
            _ball = MovementData.Ball;
        }

        internal void FixedUpdate()
        {
            if (MovementData.RedSelectedPlayer.name == PlayerData.name || MovementData.BlueSelectedPlayer.name == PlayerData.name)
                return;

            PlayerData.Target = new (_ball.transform.position.x, 0, _ball.transform.position.z);

            if (Input.GetKeyDown(KeyCode.Y))
                GoalKeeperLogic();

           // if (Vector3.Distance(PlayerData.Torso.transform.position, _ball.transform.position) < 4f)
           // {
             /*   if (Jumping)
                    GoalKeeperLogic();*/
            //}

            if (Jumping)
                ReturnToGoal();
        }

        Vector3 directionToBall;
        Vector3 pos;

        void ReturnToGoal()
        {
            if(Vector3.Distance(PlayerData.PlayerPosition, _ball.transform.position) > 0.5f)
                PlayerData.Movement = (_startingPos - PlayerData.PlayerPosition).normalized;
        }

        async void GoalKeeperLogic()
        {
            if (Vector3.Distance(PlayerData.PlayerPosition, _ball.transform.position) > 12f || PlayerData.KnockedDown)
                return;

            if (MovementController.InterceptionDirection(_ball.transform.position, PlayerData.PlayerPosition, _ball.GetComponent<Rigidbody>().velocity, 15, out _, out var result))
            {
                directionToBall = new Vector2(result.z - PlayerData.PlayerPosition.z, result.y - PlayerData.PlayerPosition.y).normalized;
                pos = result;
            }
            else
                return;
            Jumping = false;

            PlayerData.Torso.GetComponent<Rigidbody>().AddForce(new Vector3(0, directionToBall.y + .4f, directionToBall.x) * _force, ForceMode.Impulse);
            PlayerData.Movement = directionToBall;
            PlayerData.signal.Get<RagdollSignal>().Dispatch(1);
            await Task.Delay(400);
            PlayerData.Movement = Vector3.zero;
            Jumping = true;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(PlayerData.PlayerPosition + directionToBall, .5f);
        }
    }
}
