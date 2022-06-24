// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers
{

    /// <summary>
    /// A UIInputField can refer to a UI.Text or TMPro.TextMeshProUGUI.
    /// </summary>
    [Serializable]
    public class UIInputField
    {

        [SerializeField]
        private UnityEngine.UI.InputField m_uiInputField;

        /// <summary>
        /// The UI.Text assigned to this UI text field.
        /// </summary>
        public UnityEngine.UI.InputField uiInputField
        {
            get { return m_uiInputField; }
            set { m_uiInputField = value; }
        }

#if TMP_PRESENT
        [SerializeField]
        private TMPro.TMP_InputField m_textMeshProInputField;

        /// <summary>
        /// The TextMeshProUGUI assigned to this UI text field.
        /// </summary>
        public TMPro.TMP_InputField textMeshProInputField
        {
            get { return m_textMeshProInputField; }
            set { m_textMeshProInputField = value; }
        }
#endif

        /// <summary>
        /// The text content of the UI.Text or TextMeshProUGUI.
        /// </summary>
        public string text
        {
            get
            {
#if TMP_PRESENT
                if (textMeshProInputField != null) return textMeshProInputField.text;
#endif
                if (uiInputField != null) return uiInputField.text;
                return string.Empty;
            }
            set
            {
#if TMP_PRESENT
                if (textMeshProInputField != null) textMeshProInputField.text = value;
#endif
                if (uiInputField != null) uiInputField.text = value;
            }
        }

        public int characterLimit
        {
            get
            {
#if TMP_PRESENT
                if (textMeshProInputField != null) return textMeshProInputField.characterLimit;
#endif
                if (uiInputField != null) return uiInputField.characterLimit;
                return 0;
            }
            set
            {
#if TMP_PRESENT
                if (textMeshProInputField != null) textMeshProInputField.characterLimit = value;
#endif
                if (uiInputField != null) uiInputField.characterLimit = value;
            }
        }

        public bool enabled
        {
            get
            {
#if TMP_PRESENT
                if (textMeshProInputField != null) return textMeshProInputField.enabled;
#endif
                if (uiInputField != null) return uiInputField.enabled;
                return false;
            }
            set
            {
#if TMP_PRESENT
                if (textMeshProInputField != null) textMeshProInputField.enabled = value;
#endif
                if (uiInputField != null) uiInputField.enabled = value;
            }
        }

        public UIInputField()
        {
            this.uiInputField = null;
#if TMP_PRESENT
            this.textMeshProInputField = null;
#endif
        }

        public UIInputField(UnityEngine.UI.InputField uiInputField)
        {
            this.uiInputField = uiInputField;
#if TMP_PRESENT
            this.textMeshProInputField = null;
#endif
        }

#if TMP_PRESENT
        public UIInputField(TMPro.TMP_InputField textMeshProInputField)
        {
            this.uiInputField = null;
            this.textMeshProInputField = textMeshProInputField;
        }
#endif

        public GameObject gameObject
        {
            get
            {
#if TMP_PRESENT
                if (textMeshProInputField != null) return textMeshProInputField.gameObject;
#endif
                return (uiInputField != null) ? uiInputField.gameObject : null;
            }
        }

        public bool isActiveSelf { get { return (gameObject != null) ? gameObject.activeSelf : false; } }

        public bool activeInHierarchy { get { return (gameObject != null) ? gameObject.activeInHierarchy : false; } }

        public void SetActive(bool value)
        {
            if (uiInputField != null) uiInputField.gameObject.SetActive(value);
#if TMP_PRESENT
            if (textMeshProInputField != null) textMeshProInputField.gameObject.SetActive(value);
#endif
        }

        public void ActivateInputField()
        {
            if (uiInputField != null) uiInputField.ActivateInputField();
#if TMP_PRESENT
            if (textMeshProInputField != null) textMeshProInputField.ActivateInputField();
#endif
        }

        /// <summary>
        /// Checks if a UI element is assigned to a UITextField.
        /// </summary>
        /// <param name="uiTextField">UITextField to check.</param>
        /// <returns>`true` if no UI element is assigned; otherwise `false`.</returns>
        public static bool IsNull(UITextField uiTextField)
        {
            if (uiTextField == null) return true;
            if (uiTextField.uiText != null) return false;
#if TMP_PRESENT
            if (uiTextField.textMeshProUGUI != null) return false;
#endif
            return true;
        }

    }
}
