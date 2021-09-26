using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DreamersInc.MagicSkill;

namespace DreamersInc.Utils
{

    public class DragDropGridMap : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private RectTransform rectTransform;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private HorizontalLayoutGroup HLG;
        private Transform parent;
        public bool added;
        public GridPlaceCADSO GridSO;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!canvasGroup) { 
            canvasGroup = GetComponent<CanvasGroup>();
            }
            canvasGroup.alpha = .6f;
            added = false;
            canvasGroup.blocksRaycasts = false;
           
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!added) {
                rectTransform.SetParent(parent);
                HLG.SetLayoutHorizontal();
                HLG.SetLayoutVertical();

            }
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1.0f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {


        }

        // Start is called before the first frame update
        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            HLG = GetComponentInParent<HorizontalLayoutGroup>();
            parent = transform.parent;
        }

    }
}