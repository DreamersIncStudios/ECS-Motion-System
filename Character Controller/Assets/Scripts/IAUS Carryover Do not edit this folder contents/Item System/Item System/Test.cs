using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem;
public class Test : MonoBehaviour
{
    void Start()
    {
        int itemId = 2;
        BaseItem item = ItemDatabase.GetItem(itemId);
        if (item != null)
        {
            Debug.Log(string.Format("Item ID: {0}, Name: {1}, Description: {2}",
                item.ItemID, item.Name, item.Description));
        }
        else
        {
            Debug.Log(string.Format("No item with item id of {0} found", itemId));
        }
    }
}
