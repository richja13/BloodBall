using UnityEngine;

namespace Assets.Scripts
{
    internal class BallView : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "BlueGoal")
                MatchView.Instance.BlueScore++;

            if (other.gameObject.tag == "RedGoal")
                MatchView.Instance.RedScore++;
        }
    }
}
