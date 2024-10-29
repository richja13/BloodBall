using Core;
using Core.Signal;
using Football;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public InputActions actions;
    public Vector2 move;
    public Rigidbody Weapon;
    public Rigidbody Ball;
    public PlayerData player;
   // public WeaponView view;
    public bool Attack = true;
    public static TestController Instance;

    void Start()
    {
        Instance = this;
        //view = Weapon.gameObject.AddComponent<WeaponView>();
       // view.controller = this;
        actions = new InputActions();
        actions.Enable();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ball.AddForce(new Vector3(-1f, 0.2f, 0f) * 15, ForceMode.Impulse);
            Signals.Get<BallShootSignal>().Dispatch();
        }

        if (Input.GetKey(KeyCode.R))
        {
            Ball.velocity = Vector3.zero;
            Ball.transform.position = new Vector3(-2.51f, 0.98f, 6.18f);
        }

        var a = actions.GamePlay;

        Ball.transform.position = new Vector3(Ball.transform.position.x + a.RedMovement.ReadValue<Vector2>().x * .1f, Ball.transform.position.y, Ball.transform.position.z);

        if (gameObject.CompareTag("RedPlayer"))
        {
            move = a.RedMovement.ReadValue<Vector2>();
            if (a.Attack.ReadValue<float>() > 0)
                transform.Find("Animated").GetComponent<Animator>().SetTrigger("attack");
        }
        else
            move = a.RedMovement.ReadValue<Vector2>();

        player.Movement = new Vector3(move.x, 0, move.y);
    }
}



    
