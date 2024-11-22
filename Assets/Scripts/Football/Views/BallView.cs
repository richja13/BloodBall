using Football.Controllers;
using UnityEngine;
using static Football.Controllers.BallController;

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

        void Start() => hitGoal += Goal;

        internal void CustomUpdate()
        {
            if (transform.position.y < 0)
                transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }

        void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.CompareTag("FieldEnd"))
                FieldEndHit(other, transform);
        }

        void OnCollisionEnter(Collision collision) => CheckGoal(collision);

        void OnDestroy() => hitGoal -= Goal;
    }
}
