using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FNI;
namespace FNI
{
    public class IS_SetColor : MonoBehaviour
    {
        private Graphic Graphic
        {
            get
            {
                if (m_graphic == null)
                    m_graphic = GetComponent<Graphic>();

                return m_graphic;
            }
        }
        private Graphic m_graphic;

        public Color sColor = Color.white;
        public Color eColor = Color.black;
        public float sAlpha = 0;
        public float eAlpha = 1;

        public void SetColor(float value)
        {
            Graphic.color = Color.Lerp(sColor, eColor, value);
        }
        public void SetAlpha(float value)
        {
            Graphic.color = new Color(Graphic.color.r, Graphic.color.g, Graphic.color.b, Mathf.Lerp(sAlpha, eAlpha, value));
        }
    }
}
