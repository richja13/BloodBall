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
        bool _attack { get { return Controller.Attack; } set { Controller.Attack = value; } }
        string TeamTag;

        void Start()
        {
            TeamTag = Controller.playerTeam == Team.Red ? "BluePlayer" : "RedPlayer";
            Controller.OnWeaponAttack += Attack;
        }

        async void Attack()
        {
            if (_attack)
                return;

            _attack = true;
            await Task.Delay(1000);
            _attack = false;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!_attack)
                return;

            if (collision.gameObject.CompareTag(TeamTag) && collision.gameObject)
            {
                collision.gameObject.GetComponentInParent<PhysicsModule>().ActiveRagdoll.ToggleRagdoll(3);
                var data = collision.gameObject.GetComponentInParent<PlayerData>();
                data.InvokeDamage(5);
                data.HitParticles.transform.position = collision.contacts[0].point;
                data.HitParticles.transform.eulerAngles = -collision.contacts[0].normal;
                data.HitParticles.Play();
                collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.contacts[0].normal * 200, ForceMode.Impulse);
            }
        }
    }
}