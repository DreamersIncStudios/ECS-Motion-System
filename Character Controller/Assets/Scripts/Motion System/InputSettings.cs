using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSettings : MonoBehaviour
{
    public PlatformOptions TargetPlatform;
    public ControllerScheme UserScheme;
#if UNITY_STANDALONE_WIN
    public bool Controller; // if true input will be based on controller

#endif


    public void Start()
    {
#if UNITY_STANDALONE_WIN
        TargetPlatform = PlatformOptions.PC;
#endif
#if UNITY_XBOXONE
        TargetPlatform = PlatformOptions.XBOX;
#endif
#if UNITY_PS4
        TargetPlatform = PlatformOptions.PC;
#endif

        switch (TargetPlatform)
        {

            case PlatformOptions.XBOX:
            UserScheme = new XboxScheme();
                break;
            case PlatformOptions.PC:
                UserScheme = new PCKeyboardScheme();
                break;
        }
    }



}

public interface ButtonConfigs {
    KeyCode Jump { get; }
    KeyCode LightAttack { get; }
    KeyCode HeavyAttack { get; }

}
public class ControllerScheme : ButtonConfigs
{
    public KeyCode Jump { get;}
    public KeyCode LightAttack { get;  }
    public KeyCode HeavyAttack { get; }
}
public class XboxScheme : ControllerScheme
{
    public new KeyCode Jump { get { return KeyCode.Joystick1Button0; } }
    public new KeyCode LightAttack { get { return KeyCode.Joystick1Button0; } }
    public new KeyCode HeavyAttack { get { return KeyCode.Joystick1Button1; } }

}

public class PCKeyboardScheme : ControllerScheme
{
    public new KeyCode Jump { get { return KeyCode.Joystick1Button0; } }
    public new KeyCode LightAttack { get { return KeyCode.Joystick1Button0; } }
    public new KeyCode HeavyAttack { get { return KeyCode.Joystick1Button1; } }

}
public enum PlatformOptions {
    PC,XBOX,PS4, Switch
}