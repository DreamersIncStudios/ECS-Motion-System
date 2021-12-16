using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Global.Menus
{
    public class ButtonShift : MonoBehaviour,ISelectHandler,IDeselectHandler
    {
        RectTransform rect;
        Vector2 startPos;
        public Vector2 ShiftValue= new Vector2(-120,0);
        public void Awake()
        {
            rect = GetComponent<RectTransform>();
            startPos = rect.anchoredPosition;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            rect.DOAnchorPos(startPos , 2);

        }

        public void OnSelect(BaseEventData eventData)
        {
            rect.DOAnchorPos(startPos + ShiftValue, 1.25f);
        }


    }
}