using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamers.InventorySystem;

[RequireComponent(typeof(BoxCollider))]

public class StoreFront : MonoBehaviour
{
    public WHO Clerk;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            Clerk.CanOpen = true;
           GameObject player = other.gameObject;
          CharacterInventory  CharacterInventory = player.GetComponent<CharacterInventory>();

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            Clerk.CanOpen = false;
        }
    }

}
[System.Serializable]
public class WHO {
    public string name;
    public List<IGivable> givables;
    public bool CanOpen = false;

}


public interface IGivable {
    public string Name { get; set; }
    public int Value { get; set; }

}