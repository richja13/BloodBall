using Football;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public InputActions actions;
    public Vector2 move;
    public Rigidbody Weapon;
    public WeaponView view;
    public bool Attack = true;
    // Start is called before the first frame update
    void Start()
    {
        view = Weapon.gameObject.AddComponent<WeaponView>();
        view.controller = this;
        actions = new InputActions();
        actions.Enable();
    }

    void Update()
    {
        var a = actions.GamePlay;
        if (gameObject.CompareTag("RedPlayer"))
        {
            move = a.RedMovement.ReadValue<Vector2>();
            if (a.Attack.ReadValue<float>() > 0)
                transform.Find("Animated").GetComponent<Animator>().SetTrigger("attack");
        }
        else
            move = a.BlueMovement.ReadValue<Vector2>();
    }

    public void HitPlayer()
    {
        Weapon.detectCollisions = true;
    }
}



    
