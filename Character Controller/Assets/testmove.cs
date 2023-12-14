using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

public class testmove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GlobalFunctions.RandomPoint(200, out var pos);
        this.GetComponent<NavMeshAgent>().SetDestination(pos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
