using Football.Controllers;
using UnityEngine;

namespace Football.Views
{
    internal class BallView : MonoBehaviour
    {
        void Start() => BallController.hitGoal += BallController.Goal;

        void OnTriggerEnter(Collider other)
        {
            BallController.CheckGoal(other);

            if (other.gameObject.CompareTag("FieldEnd"))
                BallController.FieldEndHit(other, transform);
        }

        void OnDestroy() => BallController.hitGoal -= BallController.Goal;
    }
}
