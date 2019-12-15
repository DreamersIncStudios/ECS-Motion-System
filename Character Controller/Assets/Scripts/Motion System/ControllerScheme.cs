using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InputData", menuName = "GameParts/InputField", order = 100)]
public class ControllerScheme : ScriptableObject, ButtonConfigs
{
    [SerializeField] public KeyCode _jump;
    [SerializeField] public KeyCode _lightAttack;
    [SerializeField] public KeyCode _heavyAttack;

    public KeyCode Jump { get { return _jump; } set { _jump = value; } }
    public KeyCode LightAttack { get { return _lightAttack; } set { _lightAttack = value; } }
    public KeyCode HeavyAttack { get { return _heavyAttack; } set { _heavyAttack = value; } }
}


public enum PlatformOptions
{
    PC, XBOX, PS4, Switch
}
public interface ButtonConfigs
{
    KeyCode Jump { get; set; }
    KeyCode LightAttack { get; set; }
    KeyCode HeavyAttack { get; set; }

}
