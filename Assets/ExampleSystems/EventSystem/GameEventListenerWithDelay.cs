using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TemplateProject {
    public class GameEventListenerWithDelay : GameEventListener
    {
        [SerializeField] float _delay = 1f;
        [SerializeField] UnityEvent _delayedUnityEvent;
        public override void RaiseEvent()
        {
            _unityEvent.Invoke();
            StartCoroutine(RunDelayedEvent());
        }

        private IEnumerator RunDelayedEvent()
        {
            yield return new WaitForSeconds(_delay);
            _delayedUnityEvent.Invoke();
        }
    }
}
