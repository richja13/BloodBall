using UnityEngine;
using ActiveRagdoll.Modules;
using System.Threading.Tasks;
using Core.Enums;

namespace ActiveRagdoll
{
    public class WeaponView : MonoBehaviour
    {
        internal float Damage = 15;
        internal PlayerData Controller;
        bool _attack;
        string TeamTag;

        void Start()
        {
            TeamTag = Controller.playerTeam == Team.Red ? "BluePlayer" : "RedPlayer";
            Controller.OnWeaponAttack += Attack;
        }

        async void Attack()
        {
            _attack = true;
            await Task.Delay(1500);
            _attack = false;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (_attack)
                return;

            if (collision.gameObject.CompareTag(TeamTag))
            {
                collision.gameObject.GetComponentInParent<PhysicsModule>().ActiveRagdoll.ToggleRagdoll(3);
                collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.contacts[0].normal * 200, ForceMode.Impulse);
                Debug.Log("PlayerHit");
            }
        }
    }
}