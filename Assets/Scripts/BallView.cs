using UnityEngine;

namespace Assets.Scripts
{
    internal class BallView : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "BlueGoal")
                MatchData.BlueScore++;

            if (other.gameObject.tag == "RedGoal")
                MatchData.RedScore++;
        }
    }
}
