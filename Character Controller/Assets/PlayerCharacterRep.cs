using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PlayerCharacterRep : MonoBehaviour
{
    public GameObject Character;
    // Start is called before the first frame update
    void Start()
    {
        Character = this.gameObject;
        Entity player = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
