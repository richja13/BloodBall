using UnityEngine;
using UI;
using Football;

public class SharedView : MonoBehaviour
{
    internal void Update()
    {
        UIViewModel.CustomUpdate();
        FootballViewModel.CustomUpdate();
    }

    internal void FixedUpdate()
    {
        FootballViewModel.CustomFixedUpdate();
    }
}
