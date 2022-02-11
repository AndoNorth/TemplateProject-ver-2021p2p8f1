using UnityEngine;

namespace TemplateProject {
    public class ClickToTrigger : MonoBehaviour
    {
        [SerializeField]
        GameEvent  _onClick;
        bool _clicked;

        private void OnMouseDown()
        {
            if(_clicked == false)
            {
                Clicked();
            }
        }
        void Clicked()
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            _onClick?.Invoke();
            _clicked = true;
        }
    }
}
