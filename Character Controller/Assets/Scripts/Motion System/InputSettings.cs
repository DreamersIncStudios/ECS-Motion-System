using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputSettings
{
    public PlatformOptions TargetPlatform;
    public ControllerScheme UserScheme;


    public bool Controller; // if true input will be based on controller



    public void SetUp()
    {


        switch (TargetPlatform)
        {

            case PlatformOptions.XBOX:
                UserScheme = Resources.Load<ControllerScheme>("Controller/XboxOne");

                break;
            case PlatformOptions.PC:
                if (Controller)
                {

                    UserScheme = Resources.Load<ControllerScheme>("Controller/PCXbox");
                }
                else
                {
                    UserScheme = Resources.Load<ControllerScheme>("Controller/PCKeyboard");
                }
                break;
            case PlatformOptions.PS4:
                UserScheme = Resources.Load<ControllerScheme>("Controller/PS4One");

                break;
        }
    }

}
public interface ButtonConfigs {
    KeyCode Jump { get; set; }
    KeyCode LightAttack { get; set; }
    KeyCode HeavyAttack { get; set; }

}

[CreateAssetMenu(fileName = "InputData", menuName = "GameParts/InputField", order = 100)]
public class ControllerScheme :ScriptableObject, ButtonConfigs
{
    [SerializeField] public KeyCode _jump;
    [SerializeField] public KeyCode _lightAttack;
    [SerializeField] public KeyCode _heavyAttack;

   public KeyCode Jump { get { return _jump;}set { _jump = value; } }
   public KeyCode LightAttack { get { return _lightAttack; } set { _lightAttack = value; } }
   public KeyCode HeavyAttack { get { return _heavyAttack; } set { _heavyAttack = value; } }
}


public enum PlatformOptions {
    PC,XBOX,PS4, Switch
}