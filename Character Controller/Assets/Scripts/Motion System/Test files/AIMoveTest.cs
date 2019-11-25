using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIMoveTest : MonoBehaviour
{
    public NavMeshAgent Agent;
    public Transform Pos;
    // Start is called before the first frame update
    void Start()
    {
        if (Agent == null) {
            Agent = this.GetComponent<NavMeshAgent>();
        }

        Agent.SetDestination(Pos.position);
    }

    // Update is called once per frame

}
