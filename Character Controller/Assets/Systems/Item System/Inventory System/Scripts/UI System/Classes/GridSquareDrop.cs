using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DreamersInc.MagicSkill;
namespace DreamersInc.Utils
{
    public class GridSquareDrop : MonoBehaviour, IDropHandler
    {
        private Vector2Int position;
        private CastingDevice CAD;
        public void OnDrop(PointerEventData eventData)
        {
           if( CAD.grid.GetGridObject(position).AddMapToGrid(eventData.pointerDrag.GetComponent<DragDropGridMap>().GridSO.Grid))
            Destroy(eventData.pointerDrag);
            
        }

        public void Create(Vector2Int position, CastingDevice CAD) {
            this.position = position;
            this.CAD = CAD;
        }
        private RectTransform rectTransform;
        public void Awake() {
            rectTransform = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}