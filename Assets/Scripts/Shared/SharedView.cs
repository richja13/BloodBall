using UnityEngine;
using UI;
using Football;
using Core.Data;

public class SharedView : MonoBehaviour
{
    internal void Update()
    {
        if (!MatchData.MatchStarted)
            return;

        UIViewModel.CustomUpdate();
        FootballViewModel.CustomUpdate();
    }

    internal void FixedUpdate()
    {
        FootballViewModel.CustomFixedUpdate();
    }
}
