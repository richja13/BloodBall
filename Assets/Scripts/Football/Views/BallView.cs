using Football.Controllers;
using UnityEngine;

namespace Football.Views
{
    internal class BallView : MonoBehaviour
    {
        [SerializeField]
        public ParticleSystem BallParticles;

        [SerializeField]
        public ParticleSystem GoalExplosion;

        public static BallView Instance;

        void Awake() => Instance = this;

        void Start() => BallController.hitGoal += BallController.Goal;

        void Update()
        {
            if (transform.position.y < 0)
                transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }

        void OnTriggerEnter(Collider other)
        {
            BallController.CheckGoal(other);

            if (other.gameObject.CompareTag("FieldEnd"))
                BallController.FieldEndHit(other, transform);
        }

        void OnDestroy() => BallController.hitGoal -= BallController.Goal;
    }
}
