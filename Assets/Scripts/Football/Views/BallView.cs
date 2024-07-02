using Core.Data;
using Football.Data;
using UnityEngine;

namespace Football.Views
{
    internal class BallView : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "BlueGoal")
                MatchData.BlueScore++;

            if (other.gameObject.tag == "RedGoal")
                MatchData.RedScore++;

            if (other.gameObject.tag == "FieldEnd")
            {
                var collisionPoint =  other.ClosestPoint(transform.position);
                transform.position = new Vector3(collisionPoint.x, 1, collisionPoint.z);
                MovementData.SelectedPlayer.transform.position = new Vector3(collisionPoint.x, MovementData.SelectedPlayer.transform.position.y, collisionPoint.z);
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                Debug.Log("Filed end");
            }
        }
    }
}
