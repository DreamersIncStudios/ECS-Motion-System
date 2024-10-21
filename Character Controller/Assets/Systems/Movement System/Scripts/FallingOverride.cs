using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DreamersInc.MovementSys
{
    //todo refactor for npcs using ECS.
    public class FallingOverride : MonoBehaviour
    {
        bool IsPlayer => this.gameObject.CompareTag("Player");
        private Rigidbody rb;
        private NavMeshAgent agent;
        private Animator animator;
        [SerializeField]private float fallingTime;

        private Vector3 respawnPosition;

        private static readonly int OnGround = Animator.StringToHash("OnGround");

        // Start is called before the first frame update
        void Start()
        {

            rb= GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            if(!IsPlayer )
            {
                agent = GetComponent<NavMeshAgent>();
            }
            fallingTime = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (!animator.GetBool(OnGround))
            {
                fallingTime += Time.deltaTime;
            }
            else {
                fallingTime = 0.0f;
            }

            if(fallingTime >= 3.50f)
            {
                Reset();

            }
        }
        private void Reset()
        {
            if(respawnPosition == Vector3.zero)
                respawnPosition = GameObject.FindGameObjectWithTag("Respawn").transform.position; // Todo change to finding closer Respawn point. 

            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero; ;
            if (IsPlayer) {
                transform.position = respawnPosition;
            }
            else
            {
                agent.Warp(respawnPosition);
            }
            fallingTime = 0.0f;
            rb.useGravity = true;
        }
    }
}
