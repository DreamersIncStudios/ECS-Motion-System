using System;
using UnityEngine;
using Unity.Cinemachine;

namespace DreamersStudio.CameraControlSystem
{
    public class CameraControl : MonoBehaviour
    {
        public CinemachineCamera Follow;
        public CinemachineCamera Target;
        public CinemachineTargetGroup TargetGroup;
        public static CameraControl Instance;
        public EventHandler<OnTargetingChangedEventArgs> OnTargetingChanged;
        GameObject playerCharacter;
        public class OnTargetingChangedEventArgs : EventArgs
        {
            public bool isTargeting;
        }
        public EventHandler<OnTargetChangedEventArgs> OnTargetChanged { get; set; }
        public class OnTargetChangedEventArgs : EventArgs
        {
            public GameObject Target;

            public OnTargetChangedEventArgs()
            {
            }

            public OnTargetChangedEventArgs(GameObject target)
            {
                Target = target;
            }
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }
        private void Start()
        {
            OnTargetingChanged += (object sender, OnTargetingChangedEventArgs eventArgs) =>
            {
                if (eventArgs.isTargeting && Target.Priority != 15)
                {
                    Follow.Priority = 5;
                    Target.Priority = 15;
                }

                if (!eventArgs.isTargeting && Target.Priority == 15)
                {
                    Follow.Priority = 15;
                    Target.Priority = 5;
                }
            };
            OnTargetChanged += (object sender, OnTargetChangedEventArgs eventArgs) =>
            {
                if (eventArgs.Target.transform != null)
                {
                    TargetGroup.Targets[0].Object = eventArgs.Target.transform;
                }

                
            };

        }
        private void Update()
        {
            SetBias();
            if(!TargetGroup.Targets[0].Object )
                OnTargetingChanged(this, new CameraControl.OnTargetingChangedEventArgs { isTargeting = false });

        }
        void SetBias() {
            if (playerCharacter == null)
            {
                playerCharacter = GameObject.FindGameObjectWithTag("Player");
                return;
            }
          // Target.m_Heading.m_Bias = playerCharacter.transform.eulerAngles.y;
            
        }
    }
}
