using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Unity.Mathematics;

namespace CameraObrit
{
    public class OrbitController : MonoBehaviour
    {
        public Transform FollowPoint { get { return Camera.Follow; } }
        Vector3 preset = new Vector3(13, 0, 0);
        CinemachineVirtualCamera Camera;
        Animator anim { get { return FollowPoint.GetComponentInParent<Animator>(); } }
        public float rotationPower = .5f;
        private void Awake()
        {
            Camera = this.GetComponent<CinemachineVirtualCamera>();

        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float h = Input.GetAxis("Camera Horizontal");
            float v = Input.GetAxis("Camera Vertical");

            FollowPoint.rotation *= Quaternion.AngleAxis(h * rotationPower, Vector3.up);
            FollowPoint.rotation *= Quaternion.AngleAxis(v * rotationPower, Vector3.right);

            var angles = FollowPoint.localEulerAngles;
            angles.z = 0;
            var angle = FollowPoint.localEulerAngles.x;

            if (angle > 180 && angle < 300)
            {
                angles.x = 300;
            }
            else if (angle < 180 && angle > 80) {
                angles.x = 80;
            }
            FollowPoint.localEulerAngles = angles;

            if (Mathf.Abs(anim.GetFloat("Forward")) > 0.1f || Mathf.Abs(anim.GetFloat("Turn")) > 0.1f)
            {
                if (Mathf.Abs(angles.y) > 1.5f)
                {
                    FollowPoint.rotation = Quaternion.Lerp(FollowPoint.rotation, Quaternion.Euler(preset), Time.deltaTime*2.5f);
                }
            }
            if (Input.GetKey(KeyCode.JoystickButton9))
                FollowPoint.localEulerAngles = preset;
        }
    }
}