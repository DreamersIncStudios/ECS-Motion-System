// Copyright (c) Pixel Crushers. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrushers
{

    /// <summary>
    /// Manages a UI panel. When the panel is active and on top, it ensures that one of 
    /// its Selectables is selected if using joystick or keyboard.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UIPanel : MonoBehaviour
    {

        [Tooltip("When enabling the panel, select this if input device is Joystick or Keyboard.")]
        public GameObject firstSelected;

        [Tooltip("If non-zero, seconds between checks to ensure that one of the panel's Selectables is focused when this panel is active and on top.")]
        public float focusCheckFrequency = 0.2f;

        [Tooltip("If non-zero, refresh list of Selectables at this frequency when this panel is active and on top. Use if Selectables are added dynamically.")]
        public float refreshSelectablesFrequency = 0;

        [Tooltip("Reselect previous selectable when disabling this panel.")]
        public bool selectPreviousOnDisable = true;

        [Tooltip("When opening, set this animator trigger.")]
        public string showAnimationTrigger = "Show";

        [Tooltip("When closing, set this animator trigger.")]
        public string hideAnimationTrigger = "Hide";

        public enum StartState { GameObjectState, Open, Closed }

        [Tooltip("Normally the panel considers itself open at start if the GameObject starts active (GameObjectState). To explicitly specify whether the panel should start open or closed, select Open or Closed from the dropdown.")]
        public StartState startState = StartState.GameObjectState;

        [Tooltip("Deactivate panel GameObject when panel is closed.")]
        [SerializeField]
        protected bool m_deactivateOnHidden = true;
        public bool deactivateOnHidden
        {
            get { return m_deactivateOnHidden; }
            set { m_deactivateOnHidden = value; }
        }

        public UnityEvent onOpen = new UnityEvent();
        public UnityEvent onClose = new UnityEvent(); // Called when close starts.
        public UnityEvent onClosed = new UnityEvent(); // Called when close ends.
        public UnityEvent onBackButtonDown = new UnityEvent();

        protected GameObject m_previousSelected = null;
        protected GameObject m_lastSelected = null;
        protected List<GameObject> selectables = new List<GameObject>();
        private float m_timeNextCheck = 0;
        private float m_timeNextRefresh = 0;

        /// <summary>
        /// If false, turns off checking of current selection to make sure a valid selectable is selected.
        /// You can temporarily set this false if you open a non-UIPanel window and don't want
        /// any UIPanels to steal focus.
        /// </summary>
        public static bool monitorSelection = true;

        protected static List<UIPanel> panelStack = new List<UIPanel>();

        protected static UIPanel topPanel
        {
            get { return (panelStack.Count > 0) ? panelStack[panelStack.Count - 1] : null; }
        }

        public enum PanelState { Uninitialized, Opening, Open, Closing, Closed }

        private PanelState m_panelState = PanelState.Uninitialized;
        public PanelState panelState
        {
            get { return m_panelState; }
            set { m_panelState = value; }
        }

        public virtual bool waitForShowAnimation { get { return false; } }

        public bool isOpen
        {
            get { return panelState == PanelState.Opening || panelState == PanelState.Open || (panelState == PanelState.Uninitialized && gameObject.activeInHierarchy); }
        }

        private UIAnimatorMonitor m_animatorMonitor = null;
        public UIAnimatorMonitor animatorMonitor
        {
            get
            {
                if (m_animatorMonitor == null) m_animatorMonitor = new UIAnimatorMonitor(gameObject);
                return m_animatorMonitor;
            }
        }

        protected virtual void Start()
        {
            if (panelState == PanelState.Uninitialized)
            {
                switch (startState)
                {
                    case StartState.Open:
                        panelState = PanelState.Opening;
                        gameObject.SetActive(true);
                        RefreshSelectablesList();
                        animatorMonitor.SetTrigger(showAnimationTrigger, OnVisible, false);
                        break;
                    case StartState.Closed:
                        panelState = PanelState.Closed;
                        if (deactivateOnHidden) gameObject.SetActive(false);
                        break;
                    default:
                        if (gameObject.activeInHierarchy)
                        {
                            panelState = PanelState.Opening;
                            RefreshSelectablesList();
                            animatorMonitor.SetTrigger(showAnimationTrigger, OnVisible, false);
                        }
                        else
                        {
                            panelState = PanelState.Closed;
                        }
                        break;
                }
            }
        }

        public void RefreshSelectablesList()
        {
            selectables.Clear();
            foreach (var selectable in GetComponentsInChildren<UnityEngine.UI.Selectable>())
            {
                if (selectable.IsActive() && selectable.IsInteractable())
                {
                    selectables.Add(selectable.gameObject);
                }
            }
        }

        public void RefreshAfterOneFrame()
        {
            StartCoroutine(RefreshAfterOneFrameCoroutine());
        }

        private IEnumerator RefreshAfterOneFrameCoroutine()
        {
            yield return null;
            RefreshSelectablesList();
        }

        protected void PushToPanelStack()
        {
            if (panelStack.Contains(this)) panelStack.Remove(this);
            panelStack.Add(this);

        }

        protected void PopFromPanelStack()
        {
            panelStack.Remove(this);
        }

        /// <summary>
        /// Move this panel to the top of the stack.
        /// </summary>
        public void TakeFocus()
        {
            PushToPanelStack();
            RefreshSelectablesList();
            CheckFocus();
        }

        protected virtual void OnEnable()
        {
            PushToPanelStack();
            RefreshAfterOneFrame();
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            if (selectPreviousOnDisable && InputDeviceManager.autoFocus && UnityEngine.EventSystems.EventSystem.current != null && m_previousSelected != null && !selectables.Contains(m_previousSelected))
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(m_previousSelected);
            }
            PopFromPanelStack();
        }

        public virtual void Open()
        {
            if (panelState == PanelState.Open || panelState == PanelState.Opening) return;
            panelState = PanelState.Opening;
            gameObject.SetActive(true);
            onOpen.Invoke();
            animatorMonitor.SetTrigger(showAnimationTrigger, OnVisible, waitForShowAnimation);

            // With quick panel changes, panel may not reach OnEnable/OnDisable before being reused.
            // Update panelStack here also to handle this case:
            PushToPanelStack();
        }

        public virtual void Close()
        {
            PopFromPanelStack();
            if (gameObject.activeInHierarchy) CancelInvoke();
            if (panelState == PanelState.Closed || panelState == PanelState.Closing) return;
            panelState = PanelState.Closing;
            onClose.Invoke();
            animatorMonitor.SetTrigger(hideAnimationTrigger, OnHidden, true);

            // Deselect ours:
            if (UnityEngine.EventSystems.EventSystem.current != null && selectables.Contains(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject))
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public virtual void SetOpen(bool value)
        {
            if (value == true) Open(); else Close();
        }

        public virtual void Toggle()
        {
            if (isOpen) Close(); else Open();
        }

        protected virtual void OnVisible()
        {
            panelState = PanelState.Open;
            RefreshSelectablesList();

            // Deselect the previous selection if it's not ours:
            m_previousSelected = (UnityEngine.EventSystems.EventSystem.current != null) ? UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject : null;
            if (InputDeviceManager.autoFocus && firstSelected != null && m_previousSelected != null && !selectables.Contains(m_previousSelected))
            {
                var previousSelectable = m_previousSelected.GetComponent<UnityEngine.UI.Selectable>();
                if (previousSelectable != null) previousSelectable.OnDeselect(null);
            }
        }

        protected virtual void OnHidden()
        {
            panelState = PanelState.Closed;
            if (deactivateOnHidden) gameObject.SetActive(false);
            onClosed.Invoke();
        }

        protected virtual void Update()
        {
            if (!(isOpen && topPanel == this)) return;
            if (InputDeviceManager.isBackButtonDown)
            {
                onBackButtonDown.Invoke();
            }
            else
            {
                m_lastSelected = (UnityEngine.EventSystems.EventSystem.current != null) ? UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject : null;
                if (Time.realtimeSinceStartup >= m_timeNextCheck && focusCheckFrequency > 0 && topPanel == this && InputDeviceManager.autoFocus)
                {
                    m_timeNextCheck = Time.realtimeSinceStartup + focusCheckFrequency;
                    CheckFocus();
                }
                if (Time.realtimeSinceStartup >= m_timeNextRefresh && refreshSelectablesFrequency > 0 && topPanel == this && InputDeviceManager.autoFocus)
                {
                    m_timeNextRefresh = Time.realtimeSinceStartup + refreshSelectablesFrequency;
                    RefreshSelectablesList();
                }
            }
        }

        public virtual void CheckFocus()
        {
            if (!monitorSelection) return;
            if (!InputDeviceManager.autoFocus) return;
            if (UnityEngine.EventSystems.EventSystem.current == null) return;
            if (topPanel != this) return;
            var currentSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if (currentSelected == null || !selectables.Contains(currentSelected))
            {
                GameObject selectableToFocus = null;
                if (m_lastSelected != null && selectables.Contains(currentSelected))
                {
                    selectableToFocus = m_lastSelected;
                }
                else
                {
                    var firstSelectable = (firstSelected != null) ? firstSelected.GetComponent<UnityEngine.UI.Selectable>() : null;
                    var isFirstInteractive = firstSelectable != null && firstSelectable.IsActive() && firstSelectable.IsInteractable();
                    selectableToFocus = isFirstInteractive ? firstSelected : GetFirstInteractableButton();
                }
                if (selectableToFocus != null)
                {
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(selectableToFocus);
                }
            }
        }

        protected GameObject GetFirstInteractableButton()
        {
            foreach (var selectable in GetComponentsInChildren<UnityEngine.UI.Selectable>())
            {
                if (selectable.interactable) return selectable.gameObject;
            }
            return null;
        }

    }

}