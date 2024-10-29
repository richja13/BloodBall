using Core.Enums;
using Football.Data;
using UnityEngine;

namespace Football.Views
{
    public class PlayerOffsideIndicatorView : MonoBehaviour
    {
        [SerializeField]
        Texture2D _icon;

        [Range(20,80)]
        public float IconSize;

        [HideInInspector]
        GUIStyle _arrow;

        Vector2 _indRange;
        float _scaleRes = Screen.width / 500;
        [SerializeField]
        Camera _cam;

        public bool Visible = false; 
        PlayerData _player;

        [SerializeField]
        Team _team;

        void Start()
        {
            _arrow = new GUIStyle();
            _indRange.x = Screen.width - (Screen.width / 3);
            _indRange.y = Screen.height - (Screen.height / 4);
            _indRange /= 2f;
            _arrow.normal.textColor = new Vector4(0, 0, 0, 0); 
         
        }

        void Update()
        {
            _player = (_team == Team.Red) ? MovementData.RedSelectedPlayer.GetComponent<PlayerData>() : MovementData.BlueSelectedPlayer.GetComponent<PlayerData>();
            Vector3 screenPos = _cam.WorldToScreenPoint(_player.PlayerPosition);
            Visible = screenPos.x <= 0 || screenPos.x > Screen.width || screenPos.y <= 0 || screenPos.y >= Screen.height;
        }

        void OnGUI()
        {
            if (Visible)
            {
                Vector3 dir = _player.PlayerPosition - _cam.transform.position;
                dir = Vector3.Normalize(dir);
                dir.y *= -.5f;

                Vector2 indPos = new Vector2(_indRange.x * dir.x, _indRange.y * dir.y);
                indPos = new Vector2((Screen.width / 2) + indPos.x,
                    (Screen.height / 2) + indPos.y);

                Vector3 pdir = _player.PlayerPosition - _cam.ScreenToWorldPoint(new Vector3(indPos.x, indPos.y,
                    _player.PlayerPosition.z));
                pdir = Vector3.Normalize(pdir);

                float angle = Mathf.Atan2(pdir.x, pdir.y) * Mathf.Rad2Deg;

                GUIUtility.RotateAroundPivot(angle, indPos); 
                GUI.Box(new Rect(indPos.x, indPos.y, _scaleRes * IconSize, _scaleRes * IconSize), _icon, _arrow);
                GUIUtility.RotateAroundPivot(0, indPos); 
            }
        }
    }
}