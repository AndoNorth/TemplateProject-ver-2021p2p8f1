using UnityEngine;
using System;

namespace TemplateProject
{
    public class PoolObject : MonoBehaviour
    {
        private Action<PoolObject> _killAction;

        public void Init(Action<PoolObject> killAction)
        {
            _killAction = killAction;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.CompareTag("Ground"))
            {
                _killAction(this);
            }
        }
    }
}
