// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// This script provides methods to control InputDeviceManager that you can
    /// hook up in scripts where the InputDeviceManager instance isn't accessible
    /// at design time.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class InputDeviceMethods : MonoBehaviour
    {

        public void UseJoystick()
        {
            if (InputDeviceManager.instance == null) return;
            InputDeviceManager.instance.SetInputDevice(InputDevice.Joystick);
        }

        public void UseKeyboard()
        {
            if (InputDeviceManager.instance == null) return;
            InputDeviceManager.instance.SetInputDevice(InputDevice.Keyboard);
        }

        public void UseMouse()
        {
            if (InputDeviceManager.instance == null) return;
            InputDeviceManager.instance.SetInputDevice(InputDevice.Mouse);
        }

        public void UseTouch()
        {
            if (InputDeviceManager.instance == null) return;
            InputDeviceManager.instance.SetInputDevice(InputDevice.Touch);
        }

        public void SetCursor(bool visible)
        {
            if (InputDeviceManager.instance == null) return;
            InputDeviceManager.instance.SetCursor(visible);
        }

        public void ForceCursor(bool visible)
        {
            if (InputDeviceManager.instance == null) return;
            InputDeviceManager.instance.ForceCursor(visible);
        }

        public void BrieflyIgnoreMouseMovement()
        {
            if (InputDeviceManager.instance == null) return;
            InputDeviceManager.instance.BrieflyIgnoreMouseMovement();
        }

        public void AllowInput(bool value)
        {
            InputDeviceManager.isInputAllowed = value;
        }
    }
}