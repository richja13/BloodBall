using Core.Enums;
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
        public float PlayerSpeed;

        [SerializeField]
        [Range(4, 10)]
        public float MaxKickForce;

        [SerializeField]
        public float Agility;

        [SerializeField]
        public float Durability;

        [SerializeField]
        public PositionOnField FieldPosition;
    }
}
