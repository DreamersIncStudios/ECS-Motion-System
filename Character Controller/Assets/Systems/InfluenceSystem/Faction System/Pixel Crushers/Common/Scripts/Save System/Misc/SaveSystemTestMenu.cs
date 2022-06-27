// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers
{

    /// <summary>
    /// Simple menu for testing the Save System.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class SaveSystemTestMenu : MonoBehaviour
    {
        [Tooltip("Unity input button that toggles menu open/closed.")]
        public string menuInputButton = "Cancel";

        [Tooltip("Optional GUI Skin to provide custom Label, Button, and Box styles.")]
        public GUISkin guiSkin;

        [Tooltip("Size of menu buttons.")]
        public Vector2 buttonSize = new Vector2(200, 30);

        [Tooltip("Slot that menu saves game in.")]
        public int saveSlot = 1;

        [Tooltip("Optional instructions to show when script starts.")]
        public string instructions = "Press Escape for menu.";

        [Tooltip("How long to show instructions.")]
        public float instructionsDuration = 5;

        [Tooltip("Pause the game while the menu is open.")]
        public bool pauseWhileOpen = false;

        [Tooltip("If Input Device Manager mode is mouse, show cursor when opening and hide when closing.")]
        public bool allowCursorWhileOpen = false;

        public UnityEvent onShow = new UnityEvent();
        public UnityEvent onHide = new UnityEvent();

        private bool m_isVisible = false;
        private float m_instructionsDoneTime;
        private bool m_prevCursorState = false;

        private void Awake()
        {
            m_instructionsDoneTime = string.IsNullOrEmpty(instructions) ? 0 : Time.time + instructionsDuration;
        }

        private void Update()
        {
            if (InputDeviceManager.IsButtonDown(menuInputButton)) ToggleMenu();
        }

        public void ToggleMenu()
        {
            m_isVisible = !m_isVisible;
            if (pauseWhileOpen) Time.timeScale = m_isVisible ? 0 : 1;
            if (m_isVisible)
            {
                HandleCursor(true);
                onShow.Invoke();
            }
            else
            {
                HandleCursor(false);
                onHide.Invoke();
            }
        }

        void HandleCursor(bool open)
        {
            if (allowCursorWhileOpen && InputDeviceManager.deviceUsesCursor)
            {
                if (open)
                {
                    m_prevCursorState = Cursor.visible;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.visible = m_prevCursorState;
                    Cursor.lockState = m_prevCursorState ? CursorLockMode.None : CursorLockMode.Locked;
                }
            }
        }

        void OnGUI()
        {
            var originalSkin = GUI.skin;
            if (guiSkin != null) GUI.skin = guiSkin;

            // Draw instructions if within the timeframe to do so:
            if (Time.time < m_instructionsDoneTime)
            {
                GUILayout.Label(instructions);
            }

            // Draw menu if visible:
            if (!m_isVisible) return;
            var buttonWidth = buttonSize.x;
            var buttonHeight = buttonSize.y;
            GUILayout.BeginArea(new Rect((Screen.width - buttonWidth) / 2, (Screen.height - 4 * buttonHeight) / 2, buttonWidth, 4 * (buttonHeight + 10)));
            if (GUILayout.Button("Resume", GUILayout.Height(buttonHeight)))
            {
                ToggleMenu();
            }
            if (GUILayout.Button("Save", GUILayout.Height(buttonHeight)))
            {
                ToggleMenu();
                Debug.Log("Saving game to slot " + saveSlot);
                SaveSystem.SaveToSlot(saveSlot);
            }
            if (GUILayout.Button("Load", GUILayout.Height(buttonHeight)))
            {
                ToggleMenu();
                Debug.Log("Loading game from slot " + saveSlot);
                SaveSystem.LoadFromSlot(saveSlot);
            }
            if (GUILayout.Button("Quit", GUILayout.Height(buttonHeight)))
            {
                ToggleMenu();
                Debug.Log("Quitting");
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            GUILayout.EndArea();
            if (guiSkin != null) GUI.skin = originalSkin;
        }
    }
}
