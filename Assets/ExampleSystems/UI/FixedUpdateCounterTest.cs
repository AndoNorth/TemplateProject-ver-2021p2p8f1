using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TemplateProject {
    public class FixedUpdateCounterTest : MonoBehaviour
    {
        FixedUpdateCounter _fixedUpdateCounter;
        TextMeshProUGUI _textMeshPro;
        private void Awake()
        {
            _fixedUpdateCounter = new FixedUpdateCounter();
            _textMeshPro = transform.GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
            _textMeshPro.SetText("Fixed Updates Per Sec : " + _fixedUpdateCounter.GetCount().ToString());
        }
        private void FixedUpdate()
        {
            _fixedUpdateCounter.OnFixedUpdate();
        }
    }
}
