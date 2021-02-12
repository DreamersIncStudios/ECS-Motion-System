using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputQueuer : MonoBehaviour
{
    public Queue InputQueue;
    public bool ItemsInQueue => InputQueue.Count > 0;
    // Start is called before the first frame update
    void Start()
    {
        InputQueue = new Queue();
  
    }

   

}
