using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TemplateProject {
    namespace PlayerController2D_
    {
        namespace PlayerController2D_TopDown
        {
            public class PlayerController2D : MonoBehaviour, IMoveTransform
            {
                [SerializeField] float _moveSpeed = 10f;
                [SerializeField] float _rollSpeed = 50f;
                Vector3 _moveVector;
                Rigidbody2D _rb;
                [SerializeField] [Range(1,2)] int _moveMethod = 1;
                private void Awake()
                {
                    _rb = transform.GetComponent<Rigidbody2D>();
                }
                void Update()
                {
                    if(_moveMethod == 1)
                    {
                        MoveTransform();
                    }
                }
                private void FixedUpdate()
                {
                    if(_moveMethod == 2)
                    {
                        MoveWithVelocity();
                    }
                }
                public void SetMoveVector(Vector3 moveVector)
                {
                    _moveVector = moveVector;

                }
                private void MoveWithVelocity()
                {
                    _rb.velocity = _moveVector * _moveSpeed;

                }
                public void Roll()
                {
                    _rb.velocity = _moveVector * _rollSpeed;
                }
                private void MoveTransform()
                {
                    transform.position += _moveVector * _moveSpeed * Time.deltaTime;
                }
            }
        }

    }
}
