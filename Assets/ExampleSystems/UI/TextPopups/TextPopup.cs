using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TemplateProject
{
    public class TextPopup : MonoBehaviour
    {
        // public
        public enum TextPopupEffect
        {
            NONE = 0,
            FLOAT,
            POP,
        }
        // create a text popup
        public static TextPopup Create(Vector3 position, int text, int fontSize, Vector3 direction, TextPopupEffect textPopupEffect = TextPopupEffect.NONE, float effectStrength = 20f, float disappearTimer = 1f)
        {
            return Create(position, text.ToString(), fontSize, direction, textPopupEffect, effectStrength, disappearTimer);
        }
        public static TextPopup Create(Vector3 position, float text, int fontSize, Vector3 direction, TextPopupEffect textPopupEffect = TextPopupEffect.NONE, float effectStrength = 20f, float disappearTimer = 1f)
        {
            return Create(position, text.ToString(), fontSize, direction, textPopupEffect, effectStrength, disappearTimer);
        }
        public static TextPopup Create(Vector3 position, string text, int fontSize, Vector3 direction, TextPopupEffect textPopupEffect = TextPopupEffect.NONE, float effectStrength = 20f, float disappearTimer = 1f)
        {
            Transform textPopupTransform = Instantiate(GameAssets.instance.pfTextPopup, position, Quaternion.identity);
            TextPopup textPopup = textPopupTransform.GetComponent<TextPopup>();
            textPopup.Setup(text, fontSize, direction, textPopupEffect, effectStrength, disappearTimer);

            return textPopup;
        }
        // static
        private static int _sortingOrder;
        // const
        private const float DISAPPEAR_TIMER_MAX = 1f;
        // private
        private TextMeshPro _textMesh;
        private Color _textColor;
        private TextPopupEffect _textPopupEffect;
        // effect variables
        private float _disappearTimerInit;
        private float _disappearTimer;
        private Vector3 _moveVector;
        private float _moveVectorDecelerationRate;
        private float _effectStrength;

        private void Awake()
        {
            _textMesh = transform.GetComponent<TextMeshPro>();
            _textColor = _textMesh.color;
            _disappearTimer = 1f;
            _moveVectorDecelerationRate = 8f;
        }
        public void Setup(string text, int fontSize, Vector3 direction, TextPopupEffect textPopupEffect, float effectStrength, float disappearTimer)
        {
            _sortingOrder++;
            _textMesh.sortingOrder = _sortingOrder;

            _textMesh.SetText(text);

            _textMesh.fontSize = fontSize;

            _effectStrength = effectStrength;

            _moveVector = direction * _effectStrength;

            _textPopupEffect = textPopupEffect;

            _disappearTimerInit = disappearTimer;

            _disappearTimer = _disappearTimerInit;
        }
        private void Update()
        {
            switch (_textPopupEffect)
            {
                case TextPopupEffect.FLOAT:
                    break;
                case TextPopupEffect.POP:
                    if (_disappearTimer > _disappearTimerInit * .5f)
                    {
                        float increaseScaleAmount = 1f;
                        transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
                    }
                    else
                    {
                        float decreaseScaleAmount = 1f;
                        transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
                    }
                    break;
                case TextPopupEffect.NONE:
                default:
                    break;
            }
            // move vector in direction
            transform.position += _moveVector * Time.deltaTime;
            // reduce strength of moveVector
            _moveVector -= _moveVector * _moveVectorDecelerationRate * Time.deltaTime;
            _disappearTimer -= Time.deltaTime;
            if (_disappearTimer < 0)
            {
                // start disappearing
                float disappearSpeed = 3f;
                _textColor.a -= disappearSpeed * Time.deltaTime;
                _textMesh.color = _textColor;
                if (_textColor.a < 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
