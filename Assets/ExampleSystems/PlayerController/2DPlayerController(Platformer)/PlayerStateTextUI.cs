using UnityEngine;
using TMPro;

namespace TemplateProject
{
    public class PlayerStateTextUI : MonoBehaviour
    {
        TextMeshProUGUI _textMeshPro;
        private void Awake()
        {
            _textMeshPro = transform.GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
            _textMeshPro.SetText("Current State: ");
        }
        private void FixedUpdate()
        {

        }
    }
}
