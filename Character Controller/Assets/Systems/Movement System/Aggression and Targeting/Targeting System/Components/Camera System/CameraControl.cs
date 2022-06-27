using System;
using UnityEngine;
using Cinemachine;

namespace DreamersStudio.CameraControlSystem
{
    public class CameraControl : MonoBehaviour
    {
        public CinemachineFreeLook Follow;
        public CinemachineFreeLook Target;
        public CinemachineTargetGroup TargetGroup;
        public bool isTargeting;
        public static CameraControl Instance;
        public EventHandler<OnTargetingChangedEventArgs> OnTargetingChanged;
        public class OnTargetingChangedEventArgs : EventArgs { 
            public bool isTargeting;
        }
        public EventHandler<OnTargetChangedEventArgs> OnTargetChanged;
        public class OnTargetChangedEventArgs : EventArgs
        {
            public GameObject Target;
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
                if (eventArgs.Target != null)
                    TargetGroup.m_Targets[0].target = eventArgs.Target.transform;
            };
        }

        public void SwapFocus(Transform CharacterFocus)
        {
            Follow.Follow = CharacterFocus;
            Follow.LookAt = CharacterFocus.gameObject.GetComponentInChildren<FollowPointRef>().transform;

            Target.Follow = CharacterFocus;
            Target.LookAt = CharacterFocus.gameObject.GetComponentInChildren<FollowPointRef>().transform;
        }
    }
}