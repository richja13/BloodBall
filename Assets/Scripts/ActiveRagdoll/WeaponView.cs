using UnityEngine;
using ActiveRagdoll.Modules;

namespace ActiveRagdoll
{
    public class WeaponView : MonoBehaviour
    {
        internal float Damage = 15;
        internal PlayerData controller;

        void OnCollisionEnter(Collision collision)
        {
            if (!controller.Attack)
                return;

            if (collision.gameObject.CompareTag("BluePlayer"))
            {
                Debug.Log(collision.transform.root.name);
                collision.transform.root.GetComponent<PhysicsModule>().ActiveRagdoll.ToggleRagdoll();
                collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.contacts[0].normal * 200, ForceMode.Impulse);
                Debug.Log("PlayerHit");
            }
        }
    }
}