using UnityEngine;
using Unity.Entities;

namespace DreamersInc.ComboSystem
{
    public partial class ComboInputSystem : SystemBase
    {
        private static void ComboInputHandling(PlayerComboComponent ComboList, Command handler, ControllerInfo PC, Animator anim)
        {
            if (!anim.IsInTransition(0) && !ComboList.Combo.ShowMovesPanel)
            {
                foreach (ComboSingle combotest in ComboList.Combo.ComboLists)
                {
                    foreach (AnimationCombo comboOption in combotest.ComboList)
                    {
                        if (handler.StateInfo.IsName(comboOption.CurrentStateName.ToString()))
                        {
                            handler.currentStateExitTime = comboOption.AnimationEndTime;
                            if (comboOption.InputAllowed(handler.StateInfo.normalizedTime))
                            {
                                AnimationTrigger trigger = comboOption.Trigger;
                                if (combotest.Unlocked && handler.QueueIsEmpty && !PC.Blockb)
                                {
                                    switch (trigger.attackType)
                                    {
                                        case AttackType.LightAttack:
                                            if (PC.LightAttackb)
                                            {
                                                handler.InputQueue.Enqueue(trigger);
                                                PC.ChargedTime = 0.0f;
                                            }
                                            break;
                                        case AttackType.HeavyAttack:
                                            if (PC.HeavyAttackb)
                                            {
                                                handler.InputQueue.Enqueue(trigger);
                                                PC.ChargedTime = 0.0f;
                                            }
                                            break;
                                        //TODO Review
                                        case AttackType.ChargedLightAttack:
                                            if (PC.ChargedLightAttackb)
                                            {
                                                handler.InputQueue.Enqueue(trigger);
                                                PC.ChargedTime = 0.0f;
                                            }
                                            break;
                                        case AttackType.ChargedHeavyAttack:
                                            if (PC.ChargedHeavyAttackb)
                                            {
                                                handler.InputQueue.Enqueue(trigger);
                                                PC.ChargedTime = 0.0f;
                                            }
                                            break;
                                        case AttackType.Projectile:
                                            if (PC.Projectileb)
                                            {
                                                handler.InputQueue.Enqueue(trigger);
                                                PC.ChargedTime = 0.0f;
                                            }
                                            break;
                                    }
                                }
                                else if (PC.Blockb)
                                {
                                    handler.InputQueue.Enqueue(new AnimationTrigger()
                                    {
                                        attackType = AttackType.Defend,

                                    });

                                }

                            }
                        }
                    }
                }
            }
        }

    }
}