using Core.Data;
using Football.Controllers;
using Football.Data;
using UnityEngine;

namespace Football.Views
{
    internal class BallView : MonoBehaviour
    {
        void Start() => BallController.hitGoal += BallController.Goal;

        void OnTriggerEnter(Collider other)
        {
            BallController.CheckGoal(other);

            if (other.gameObject.tag == "FieldEnd")
                BallController.FieldEndHit(other, transform);
        }

        void OnDestroy() => BallController.hitGoal -= BallController.Goal;
    }
}
