// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Keeps a RectTransform's bounds in view of the main camera. 
    /// Works best on world space panels.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class KeepRectTransformOnscreen : MonoBehaviour
    {
        private Vector3 originalLocalPosition;
        private RectTransform rectTransform;

        private void Awake()
        {
            originalLocalPosition = transform.localPosition;
            rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            // Reset to original position:
            transform.localPosition = originalLocalPosition;

            // Get corner bounds:
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            var rectWidth = Mathf.Abs(corners[2].x - corners[0].x);
            var rectHeight = Mathf.Abs(corners[2].y - corners[0].y);

            // Get screen bounds:
            var bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
            var topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));
            var cameraRect = new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);

            // Clamp:
            var validRect = new Rect(cameraRect.x + (rectWidth / 2), cameraRect.y, cameraRect.width - rectWidth, cameraRect.height - rectHeight);
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, validRect.xMin, validRect.xMax),
                Mathf.Clamp(transform.position.y, validRect.yMin, validRect.yMax),
                transform.position.z);
        }
    }
}