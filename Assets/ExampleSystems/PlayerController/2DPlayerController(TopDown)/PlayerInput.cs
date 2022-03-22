using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TemplateProject
{
    namespace PlayerController2D_
    {
        namespace PlayerController2D_TopDown
        {
            public class PlayerInput : MonoBehaviour
            {
                Vector2 _inputDirection = new Vector2(0f, 0f);
                bool _rollThisFrame = false;
                IMoveTransform _moveTransform;
                Vector3 _movePosition;
                [SerializeField] [Range(1, 2)] int _inputMethod = 1;
                private void Start()
                {
                    _moveTransform = transform.GetComponent<IMoveTransform>();
                }
                void Update()
                {
                    if (_inputMethod == 1)
                    {
                        _inputDirection = Vector2.zero;
                        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                        {
                            _inputDirection.y += 1;

                        }
                        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                        {
                            _inputDirection.y -= 1;
                        }
                        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                        {
                            _inputDirection.x += 1;

                        }
                        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                        {
                            _inputDirection.x -= 1;

                        }
                        if (Input.GetKey(KeyCode.Space))
                        {
                            _rollThisFrame = true;
                        }
                    }

                    if (_inputMethod == 2)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            SetMovePosition(GeneralUtility.GetMouseWorldPosition());
                        }
                        _inputDirection = (_movePosition - transform.position);
                        if (_inputDirection.magnitude < 1f)
                        {
                            _inputDirection = Vector3.zero;
                        }
                    }

                    _inputDirection = _inputDirection.normalized;

                    _moveTransform.SetMoveVector(new Vector3(_inputDirection.x, _inputDirection.y));
                }
                private void SetMovePosition(Vector3 movePosition)
                {
                    _movePosition = movePosition;
                }
            }
        }
    }
}
