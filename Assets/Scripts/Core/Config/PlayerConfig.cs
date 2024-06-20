using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Core/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField]
        internal string PlayerNumber;

        [SerializeField]
        internal string PlayerName;

        [SerializeField]
        public GameObject PlayerModel;

        [SerializeField]
        [Range(2, 4)]
        internal float PlayerSpeed;

        [SerializeField]
        [Range(4, 10)]
        internal float MaxKickForce;

        [SerializeField]
        internal float Agility;

        [SerializeField]
        internal float Durability;
    }
}
