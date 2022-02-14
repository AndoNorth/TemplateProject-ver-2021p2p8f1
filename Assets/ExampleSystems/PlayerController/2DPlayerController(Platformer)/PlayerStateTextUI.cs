using UnityEngine;
using TMPro;

namespace TemplateProject
{
    namespace PlayerController2D_
    {
        namespace PlayerController2D_Platformer
        {
            public class PlayerStateTextUI : MonoBehaviour
            {
                private TextMeshProUGUI _textMeshPro;
                private PlayerController2D _playerController2D;
                private string _currentState;
                private void Awake()
                {
                    _textMeshPro = transform.GetComponent<TextMeshProUGUI>();
                    _playerController2D = FindObjectOfType<PlayerController2D>();
                }
                private void FixedUpdate()
                {
                    if (_currentState == _playerController2D.CurrentState)
                    {
                        return;
                    }
                    _textMeshPro.SetText("Current State: " + _playerController2D.CurrentState + "\n"
                        + "Previous State: " + _playerController2D.PreviousState + "\n");
                    _currentState = _playerController2D.CurrentState;
                }
            }
        }
    }
}
