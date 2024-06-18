using Assets.Scripts;
using System.Threading.Tasks;
using UnityEngine;

public class MovementView : MonoBehaviour
{
    bool _shoot = false;
    bool _pass = false;
    float _kickPower = 2;
    const float MAX_KICK_POWER = 4;

    delegate void KickBall(float power, Vector3 direction);
    event KickBall kickBall;

    void Start() => kickBall += MovementController.BallAddForce;

    void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        MovementData.SelectedPlayer.transform.position += MovementController.Movement(x, y, MovementData.BasicSpeed);

        MovementData.SelectedPlayer.transform.rotation = ((x + y) != 0) ? Quaternion.Euler(0,MovementController.RotationY(x,y),0) : MovementData.SelectedPlayer.transform.rotation;

        if(MovementData.PlayerHasBall)
            MovementData.Ball.transform.localPosition = new Vector3(0, -0.5f, 0.85f);

        MovementController.GetBall();

        if (Input.GetMouseButton(0))
            LoadKickForce();

        if (Input.GetMouseButtonUp(0))
            _shoot = true;

        if (Input.GetMouseButtonDown(1))
            _pass = true;
    }

    void FixedUpdate()
    {
        if (_shoot)
            Shoot();

        if(_pass)
            Pass();
    }

    void LoadKickForce()
    {
        if (!MovementData.PlayerHasBall)
            return;

        _kickPower += (_kickPower < MAX_KICK_POWER) ? Time.deltaTime * 3 : 0;
        MatchView.Instance.LoadPowerBar(MatchData.RedTeamBar, MAX_KICK_POWER, _kickPower);
    }

    async void Shoot()
    {
        _shoot = false;

        MatchView.Instance.LoadPowerBar(MatchData.RedTeamBar, MAX_KICK_POWER, 0);

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

        var closestPlayer = MovementController.FindClosestPlayer(MovementData.RedTeamPlayers, MovementData.SelectedPlayer.transform, out var distance);
        MovementData.SelectedPlayer.transform.LookAt(closestPlayer);
        MovementData.Ball.transform.LookAt(MovementData.SelectedPlayer.transform);
        var vector = new Vector3(closestPlayer.position.x - MovementData.SelectedPlayer.transform.position.x, 0, closestPlayer.position.z - MovementData.SelectedPlayer.transform.position.z)/10;
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
}
