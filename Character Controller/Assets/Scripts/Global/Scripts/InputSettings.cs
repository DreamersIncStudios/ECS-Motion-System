using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
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
