using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TemplateProject
{
    public class TextPopupTest : MonoBehaviour
    {
        [SerializeField]
        Transform pfTextPopup;

        struct TextEffect
        {
            public Vector3 direction;
            public TextPopup.TextPopupEffect textPopupEffect;
            public float effectStrength;
            public float disappearTimer;
            public TextEffect(Vector3 direction, TextPopup.TextPopupEffect textPopupEffect, float effectStrength, float disappearTimer)
            {
                this.direction = direction;
                this.textPopupEffect = textPopupEffect;
                this.effectStrength = effectStrength;
                this.disappearTimer = disappearTimer;
            }
        }

        TextEffect textEffect = new TextEffect(Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 1f);
        bool randomDirection = false;
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 position = GeneralUtility.GetMouseWorldPosition();
                if (randomDirection)
                {

                    TextPopup.Create(position, "100", 16,Random.insideUnitCircle, textEffect.textPopupEffect, textEffect.effectStrength);
                }
                else
                {
                    TextPopup.Create(position, "100", 16, textEffect.direction, textEffect.textPopupEffect, textEffect.effectStrength);
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                textEffect = new TextEffect(Vector3.zero, TextPopup.TextPopupEffect.NONE, 0f, 3f);
                Debug.Log("set to config 1");
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                textEffect = new TextEffect(new Vector3(0, 1), TextPopup.TextPopupEffect.NONE, 20f, 1.5f);
                Debug.Log("set to config 2");
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                textEffect = new TextEffect(new Vector3(0, 1), TextPopup.TextPopupEffect.POP, 30f, 1f);
                Debug.Log("set to config 3");
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                randomDirection = !randomDirection;
                Debug.Log("random direction toggled");
            }
        }
    }
}
