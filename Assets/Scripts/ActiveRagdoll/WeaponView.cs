using UnityEngine;
using ActiveRagdoll.Modules;
using System.Threading.Tasks;
using Core.Enums;
using System.Linq;

namespace ActiveRagdoll
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField]
        Material WeaponMaterial;
        internal float Damage = 15;
        internal float AttackSpeed = 2;
        internal PlayerData Controller 
        { 
            get { return _controller; }

            set 
            {
                _controller = value;

                if (!value.Weapon)
                    return;

                Damage = value.Weapon.WeaponDamage;
                AttackSpeed = value.Weapon.AttackSpeed;
            }   
        }

        PlayerData _controller;

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

            if (collision.gameObject.CompareTag("Weapon"))
                AddWeaponToHand(collision.gameObject);

            if (!_attack)
                return;

            if (collision.gameObject.CompareTag(TeamTag) && collision.gameObject)
            {
                collision.gameObject.GetComponentInParent<PhysicsModule>().ActiveRagdoll.ToggleRagdoll(3);
                var data = collision.gameObject.GetComponentInParent<PlayerData>();
                data.InvokeDamage(5);
                data.HitParticles.transform.parent = data.Torso.transform;
                data.HitParticles.transform.position = collision.contacts[0].point;
                data.HitParticles.transform.eulerAngles = -collision.contacts[0].normal;
                data.HitParticles.Play();
                collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.contacts[0].normal * 200, ForceMode.Impulse);
            }
        }

        void AddWeaponToHand(GameObject collisionObj)
        {
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);

            Controller.Weapon = WeaponSpawnerView.Instance?.AllWeapons.Where(weapon => collisionObj.gameObject.name.Contains(weapon.WeaponName)).ToList()[0];
            collisionObj.gameObject.transform.parent = transform;
            collisionObj.gameObject.transform.position = transform.position;
            Destroy(collisionObj.gameObject.GetComponent<Rigidbody>());
            collisionObj.gameObject.transform.localEulerAngles = new Vector3(0, 90, 95);
            collisionObj.gameObject.GetComponent<SphereCollider>().enabled = false;
            collisionObj.gameObject.GetComponent<MeshRenderer>().material = WeaponMaterial;
            collisionObj.gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }
}