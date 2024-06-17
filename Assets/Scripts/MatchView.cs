using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    internal class MatchView : MonoBehaviour
    {
        public static MatchView Instance;

        [SerializeField]
        UIDocument document;

        [SerializeField]
        GameObject _redGoal;

        [SerializeField]
        GameObject _blueGoal;

        internal float RedScore;
        internal float BlueScore;

        double _time;

        Label UItime;

        Label UIScore;

        void Awake() => Instance = this;   

        void OnEnable()
        {
            _time = 0;
            UItime = document.rootVisualElement.Q<Label>(className: "time");
            UIScore = document.rootVisualElement.Q<Label>(className: "score");
        }

        private void Update()
        {
            UIScore.text = $"{BlueScore}:{RedScore}";
            _time  += Time.deltaTime * 4;
            var timespan = TimeSpan.FromSeconds(_time);
            UItime.text = string.Format("{0:00}:{1:00}", timespan.TotalMinutes, timespan.Seconds);
        }
    }
}
