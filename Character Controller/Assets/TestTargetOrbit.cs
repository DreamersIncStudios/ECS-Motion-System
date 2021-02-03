using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class TestTargetOrbit : MonoBehaviour
{

    public Transform Target;
    public Transform FollowPoint;
    public Vector3 dir;
    CinemachineFreeLook look;
    public float Output;

    // Start is called before the first frame update
    void Start()
    {
        look = GetComponent<CinemachineFreeLook>();
        
    }

    // Update is called once per frame
    void Update()
    {
      dir = (-FollowPoint.transform.position + Target.transform.position).normalized;

        if (dir.x >= 0)
        {
            Output = Vector3.Angle(Vector3.forward, dir);
            look.m_XAxis.Value = Output;
        }
        if (dir.x < 0)
        {
            Output = Vector3.Angle(Vector3.forward, dir);
            look.m_XAxis.Value = -Output;
        }
    }
}
