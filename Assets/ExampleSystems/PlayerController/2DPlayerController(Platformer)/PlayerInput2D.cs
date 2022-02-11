using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TemplateProject
{
    namespace PlayerController2D_
    {
        namespace PlayerController2D_Platformer
        {
            public class PlayerInput2D : MonoBehaviour
            {
                PlayerController2D _playerController2D;

                private void Start()
                {
                    _playerController2D = GetComponent<PlayerController2D>();
                }
                private void Update()
                {
                    _playerController2D.SetInputs(GetInputs());
                }
                private FrameInputs GetInputs()
                {
                    FrameInputs inputs = new FrameInputs {
                        UpDown          = Input.GetButtonDown("Horizontal"),
                        UpUp            = Input.GetButtonDown("Horizontal"),
                        DownDown        = Input.GetButtonDown("Horizontal"),
                        DownUp          = Input.GetButtonDown("Horizontal"),
                        LeftDown        = Input.GetButtonDown("Horizontal"),
                        LeftUp          = Input.GetButtonDown("Horizontal"),
                        RightDown       = Input.GetButtonDown("Horizontal"),
                        RightUp         = Input.GetButtonDown("Horizontal"),
                        JumpDown        = Input.GetButtonDown("Horizontal"),
                        JumpUp          = Input.GetButtonDown("Horizontal"),
                        LightAttackDown = Input.GetButtonDown("Horizontal"),
                        LightAttackUp   = Input.GetButtonDown("Horizontal"),
                        HeavyAttackDown = Input.GetButtonDown("Horizontal"),
                        HeavyAttackUp   = Input.GetButtonDown("Horizontal"),
                        OtherDown       = Input.GetButtonDown("Horizontal"),
                        OtherUp         = Input.GetButtonDown("Horizontal"),
                    };
                    return inputs;
                }
            }
        }
    }
}
