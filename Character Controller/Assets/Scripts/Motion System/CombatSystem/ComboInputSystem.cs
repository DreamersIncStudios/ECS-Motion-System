using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;
using MotionSystem.Components;
using IAUS.ECS.Component;
using UnityStandardAssets.CrossPlatformInput;
using Unity.Mathematics;
using System.Collections.Generic;


public class ComboInputSystem : ComponentSystem
{
    public ControllerScheme InputSet => Core.GameMaster.Instance.InputSettings.UserScheme;
    public bool Jump => Input.GetKeyUp(InputSet.Jump);
    public bool DisplayCombos => Input.GetKeyUp(KeyCode.JoystickButton7);
    public bool Block => Input.GetKeyDown(InputSet.Block);
    public bool LightAttack => Input.GetKeyUp(InputSet.LightAttack);
    public bool HeavyAttack => Input.GetKeyUp(InputSet.HeavyAttack);
    public bool ChargedLightAttack => Input.GetKeyUp(InputSet.ChargedLightAttack); // change to time base later 
    public bool ChargedHeavyAttack => Input.GetKeyUp(InputSet.ChargedHeavyAttack); // change to time base later 
    public bool Projectile => Input.GetKeyUp(InputSet.Projectile);


    Queue<AnimationTriggers> InputQueue;

    protected override void OnCreate()
    {
        base.OnCreate();
        InputQueue = new Queue<AnimationTriggers>();
        movespanel = new GameObject();
    }

    public bool QueueIsEmpty => InputQueue.Count == 0;

    float currentStateExitTime;
    AnimatorStateInfo StateInfo;
    bool TakeInput =>!QueueIsEmpty && StateInfo.normalizedTime > currentStateExitTime;
    bool TransitionToLocomotion => !StateInfo.IsTag("Locomotion") && StateInfo.normalizedTime > .95f;
    GameObject movespanel;
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Player_Control PC, ComboComponent ComboList) =>
        {
           
            StateInfo = ComboList.animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log(DisplayCombos);
            if (DisplayCombos)
            {
                ComboList.combo.ShowMovesPanel = !ComboList.combo.ShowMovesPanel;
                if (ComboList.combo.ShowMovesPanel)
                    movespanel = ComboList.combo.DisplayCombo();
                else
                    Object.Destroy(movespanel);
            }

            if (!ComboList.animator.IsInTransition(0) && !ComboList.combo.ShowMovesPanel)
            {
                foreach (AnimationCombo comboOption in ComboList.combo.ComboList)
                {
                    if (StateInfo.IsName(comboOption.CurremtStateName.ToString()))
                    {
                        currentStateExitTime = comboOption.AnimationEndTime;
                        //Light
                        if (comboOption.LightAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(StateInfo.normalizedTime))
                            {
                                if (LightAttack && QueueIsEmpty)
                                {
                                    InputQueue.Enqueue(comboOption.LightAttack);
                                }
                            }
                        }
                        //Heavy
                        if (comboOption.HeavyAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(StateInfo.normalizedTime))
                            {
                                if (HeavyAttack && QueueIsEmpty)
                                {
                                    InputQueue.Enqueue(comboOption.HeavyAttack);
                                }
                            }
                        }
                        //Charge Light
                        if (comboOption.ChargedLightAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(StateInfo.normalizedTime))
                            {
                                if (ChargedLightAttack && QueueIsEmpty)
                                {
                                    InputQueue.Enqueue(comboOption.ChargedLightAttack);
                                }
                            }
                        }
                        //Charge Heavy
                        if (comboOption.ChargedHeavyAttack.Unlocked)
                        {
                            if (comboOption.InputAllowed(StateInfo.normalizedTime))
                            {
                                if (ChargedHeavyAttack && QueueIsEmpty)
                                {
                                    InputQueue.Enqueue(comboOption.ChargedHeavyAttack);
                                }
                            }
                        }
                        //projectile
                        if (comboOption.Projectile.Unlocked)
                        {
                            if (comboOption.InputAllowed(StateInfo.normalizedTime))
                            {
                                if (Projectile && QueueIsEmpty)
                                {
                                    InputQueue.Enqueue(comboOption.Projectile);
                                }
                            }
                        }
                    }
                }

                if (TakeInput)
                {
                    AnimationTriggers temp = InputQueue.Dequeue();
                    ComboList.animator.CrossFade(temp.TriggeredAnimName.ToString(), temp.TransitionDuration, 0, temp.StartOffset);

                }

                if (TransitionToLocomotion)
                {
                    ComboList.animator.CrossFade("Locomation_Grounded_Weapon", .25f, 0, 0.25f);
                }
            }
        });

    }
}
