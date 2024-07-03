using UnityEngine;

namespace Core.Config
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/Core/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField]
        public string PlayerNumber;

        [SerializeField]
        public string PlayerName;

        [SerializeField]
        public GameObject PlayerModel;

        public Transform SpawnPoint;

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
