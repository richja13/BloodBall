using ActiveRagdoll;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    internal float Damage = 15;
    internal TestController controller;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!controller.Attack)
            return;

        if (collision.gameObject.CompareTag("BluePlayer"))
        {
            Debug.Log(collision.transform.root.name);
            collision.transform.root.GetComponent<PhysicsModule>().ActiveRagdoll.ToggleRagdoll();
            collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.contacts[0].normal * 150, ForceMode.Impulse);
            Debug.Log("PlayerHit");
        }
    }
}
