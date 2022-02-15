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
                        UpDown          = Input.GetKeyDown(KeyCode.UpArrow),
                        Up              = Input.GetKey(KeyCode.UpArrow),
                        UpUp            = Input.GetKeyUp(KeyCode.UpArrow),
                        DownDown        = Input.GetKeyDown(KeyCode.DownArrow),
                        Down            = Input.GetKey(KeyCode.DownArrow),
                        DownUp          = Input.GetKeyUp(KeyCode.DownArrow),
                        LeftDown        = Input.GetKeyDown(KeyCode.LeftArrow),
                        Left            = Input.GetKey(KeyCode.LeftArrow),
                        LeftUp          = Input.GetKeyUp(KeyCode.LeftArrow),
                        RightDown       = Input.GetKeyDown(KeyCode.RightArrow),
                        Right           = Input.GetKey(KeyCode.RightArrow),
                        RightUp         = Input.GetKeyUp(KeyCode.RightArrow),
                        JumpDown        = Input.GetKeyDown(KeyCode.Space),
                        JumpUp          = Input.GetKeyUp(KeyCode.Space),
                        LightAttackDown = Input.GetKeyDown(KeyCode.Z),
                        LightAttack     = Input.GetKey(KeyCode.Z),
                        LightAttackUp   = Input.GetKeyUp(KeyCode.Z),
                        HeavyAttackDown = Input.GetKeyDown(KeyCode.X),
                        HeavyAttack     = Input.GetKey(KeyCode.X),
                        HeavyAttackUp   = Input.GetKeyUp(KeyCode.X),
                        OtherDown       = Input.GetKeyDown(KeyCode.C),
                        Other           = Input.GetKey(KeyCode.C),
                        OtherUp         = Input.GetKeyUp(KeyCode.C),
                    };
                    return inputs;
                }
            }
        }
    }
}
