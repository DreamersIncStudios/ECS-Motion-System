using AISenses.VisionSystems.Combat;
using UnityEngine;
using Unity.Entities;
using DreamersStudio.CameraControlSystem;
using Unity.Collections;
using DreamersInc;
using DreamersInc.InputSystems;
using MotionSystem.Components;
using UnityEngine.InputSystem;

namespace AISenses.VisionSystems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class TargetingSystem : SystemBase
    {
        private PlayerControls playerControls;

        protected override void OnCreate()
        {
            RequireForUpdate<Player_Control>();
            RequireForUpdate<InputSingleton>();
            if (SystemAPI.ManagedAPI.TryGetSingleton<InputSingleton>(out var inputSingle))
            {
                playerControls = inputSingle.ControllerInput;
            }
        }

        private CameraControl cameraControl;
        protected override void OnStartRunning()
        {
            if (playerControls == null)
            {
                if (SystemAPI.ManagedAPI.TryGetSingleton<InputSingleton>(out var inputSingle))
                {
                    playerControls = inputSingle.ControllerInput;
                }
            }
            cameraControl = CameraControl.Instance;
            playerControls.PlayerController.LockOn.performed += ToggleTargeting;
            playerControls.PlayerController.ChangeTargetNeg.performed += ChangeTargetNegative;
            playerControls.PlayerController.ChangeTargetPos.performed += ChangeTargetPositive;
        }

        protected override void OnStopRunning()
        {
            playerControls.PlayerController.LockOn.performed -= ToggleTargeting;
            playerControls.PlayerController.ChangeTargetNeg.performed -= ChangeTargetNegative;
            playerControls.PlayerController.ChangeTargetPos.performed -= ChangeTargetPositive;
        }

        protected override void OnUpdate()
        {
        
        }
        int index = 0;
        void ToggleTargeting(InputAction.CallbackContext obj)
        {
            if(!cameraControl)
                cameraControl = CameraControl.Instance;
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((DynamicBuffer<ScanPositionBuffer> buffer, ref CharControllerE Control,ref AttackTarget attackTarget) =>
            {
                var sortedBuffer = buffer.AsNativeArray();
                sortedBuffer.Sort( new SortScanPositionByDistance());
                
                Control.Targetting = !Control.Targetting;
                if (cameraControl.OnTargetingChanged != null)
                {
                  cameraControl.OnTargetingChanged(this,
                        new CameraControl.OnTargetingChangedEventArgs() { isTargeting = Control.Targetting });
                }

                index = 0;
                SetTarget(sortedBuffer);
                attackTarget.AttackTargetIndex = Control.Targetting ? index : -1;
                attackTarget.IsTargeting = Control.Targetting;
                
            }).Run();
        }

        void ChangeTargetPositive(InputAction.CallbackContext obj)
        {
            if(!cameraControl)
                cameraControl = CameraControl.Instance;
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((DynamicBuffer<ScanPositionBuffer> buffer, ref CharControllerE Control, ref AttackTarget attackTarget) =>
            {     
                if(!Control.Targetting) return; 
                var sortedBuffer = buffer.AsNativeArray();
                sortedBuffer.Sort( new SortScanPositionByDistance());
                index++;
                if (index > buffer.Length - 1)
                    index = 0;
                SetTarget(sortedBuffer);
                attackTarget.AttackTargetIndex = Control.Targetting ? index : -1;
            }).Run();
        }
        void ChangeTargetNegative(InputAction.CallbackContext obj)
        {
            if(!cameraControl)
                cameraControl = CameraControl.Instance;
            Entities.WithoutBurst().WithAll<Player_Control>().ForEach((DynamicBuffer<ScanPositionBuffer> buffer, ref CharControllerE Control,  ref AttackTarget attackTarget) =>
            {      
                if(!Control.Targetting) return; 
                var sortedBuffer = buffer.AsNativeArray();
                sortedBuffer.Sort( new SortScanPositionByDistance());
                index--;
                if (index < 0)
                    index = buffer.Length - 1;
                
                attackTarget.AttackTargetIndex = Control.Targetting ? index : -1;
                SetTarget(sortedBuffer);
            }).Run();
        }

        private void SetTarget(NativeArray<ScanPositionBuffer> sortedBuffer)
        {
            if (cameraControl.OnTargetChanged != null)
            {
                cameraControl.OnTargetChanged(this,
                    new CameraControl.OnTargetChangedEventArgs(EntityManager
                        .GetComponentObject<Animator>(sortedBuffer[index].target.Entity)
                        .gameObject));
            }
        }
    }
}