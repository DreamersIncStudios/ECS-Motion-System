using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class AnimatorIK : MonoBehaviour
{
    Animator anim;
    Entity entity { get { return this.GetComponent<MotionSystem.Archetypes.CharacterControl>().ObjectEntity; } }
    public float offsetY;
    public LayerMask TargetLayers; // check AI System To see how it was used in RayCast Command 
    private void Awake()
    {
        if (anim == null)
        {
            anim = this.GetComponent<Animator>();
        }
    }

    float LeftFootWeight { get { return anim.GetFloat("Left Foot"); } }
    float RightFootWeight { get { return anim.GetFloat("Right Foot"); } }
    Vector3 LFposFinal;
    Vector3 RFposFinal;

    Quaternion LFRotFinal;
    Quaternion RFRotFinal;

    Vector3 LeftFootPos { get { return anim.GetBoneTransform(HumanBodyBones.LeftFoot).transform.TransformPoint(Vector3.zero); } }
    Vector3 RightFootPos { get { return anim.GetBoneTransform(HumanBodyBones.RightFoot).transform.TransformPoint(Vector3.zero); } }
    Quaternion LeftFootRot { get { return anim.GetBoneTransform(HumanBodyBones.LeftFoot).transform.localRotation; } }
    Quaternion RightFootRot { get { return anim.GetBoneTransform(HumanBodyBones.RightFoot).transform.localRotation; } }
    private void OnAnimatorIK(int layerIndex)
    {

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, LeftFootWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, RightFootWeight);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, LeftFootWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, RightFootWeight);

        NativeList<RaycastCommand> commands = new NativeList<RaycastCommand>(Allocator.TempJob);
        NativeArray<RaycastHit> Results = new NativeArray<RaycastHit>(2, Allocator.TempJob);
        var Job = new SetupRaysforIK() { LeftFootPos = LeftFootPos, RightFootPos = RightFootPos,commands = commands };

        JobHandle SetupJob = Job.Schedule();

        SetupJob.Complete();

        commands = Job.commands;

        JobHandle rayjob = RaycastCommand.ScheduleBatch(commands, Results, 2,SetupJob);
        rayjob.Complete();

        if (Results[0].collider != null)
        {
            Debug.Log(Results[0].collider.name);
            LFposFinal = Results[0].point;
            LFRotFinal = Quaternion.FromToRotation(transform.up, Results[0].normal) * transform.rotation;
        }
        else
        { LFposFinal = LeftFootPos;
            LFRotFinal = LeftFootRot;
        }

        if (Results[1].collider != null)
        {
            RFposFinal = Results[1].point;
            RFRotFinal = Quaternion.FromToRotation(transform.up, Results[1].normal) * transform.rotation;
        }
        else
        {
            RFposFinal = RightFootPos;
            RFRotFinal = RightFootRot;
        }


        anim.SetIKPosition(AvatarIKGoal.LeftFoot, LFposFinal);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, LFRotFinal);

        anim.SetIKPosition(AvatarIKGoal.RightFoot, RFposFinal);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, RFRotFinal);

        commands.Dispose();
        Results.Dispose();

    }

}
[BurstCompile]
public struct SetupRaysforIK : IJob
{
  
    public Vector3 LeftFootPos;
    public Vector3 RightFootPos;
    public NativeList<RaycastCommand> commands;


    public void Execute()
    {
        RaycastCommand left = new RaycastCommand() { from = LeftFootPos, direction = Vector3.down, distance = 1.0f, maxHits = 1 };
        RaycastCommand Right = new RaycastCommand() { from = RightFootPos, direction = Vector3.down, distance = 1.0f, maxHits = 1 };

    }


}



