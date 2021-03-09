using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

[GenerateAuthoringComponent]
public struct Player_Control : IComponentData {
    public ControllerScheme InputSet => Core.GameMaster.Instance.InputSettings.UserScheme;
    public bool Jump => Input.GetKeyUp(InputSet.Jump);
    public bool DisplayCombos => Input.GetKeyUp(KeyCode.JoystickButton7);
    public bool Block => Input.GetKeyDown(InputSet.Block);
    public bool LightAttack => Input.GetKeyUp(InputSet.LightAttack);
    public bool HeavyAttack => Input.GetKeyUp(InputSet.HeavyAttack);
    public bool ChargedLightAttack => Input.GetKeyUp(InputSet.ChargedLightAttack); // change to time base later 
    public bool ChargedHeavyAttack => Input.GetKeyUp(InputSet.ChargedHeavyAttack); // change to time base later 
    public bool Projectile => Input.GetKeyUp(InputSet.Projectile);


}

