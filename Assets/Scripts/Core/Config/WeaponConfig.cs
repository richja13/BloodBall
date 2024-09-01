using Core.Enums;
using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Config/Core/WeaponConfig")]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField]
        public string WeaponName;

        [SerializeField]
        public GameObject WeaponModel;

        [SerializeField]
        public WeaponType WeaponType;

        [SerializeField]
        public float WeaponDamage;

        [SerializeField]
        public int WeaponDurability;

        [SerializeField]
        public float AttackSpeed;

        [SerializeField]
        public float StunTime;
    }
}