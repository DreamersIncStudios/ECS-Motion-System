using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem;

public class Inventory : MonoBehaviour
{
    public List<BaseItem> Items;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

 public BaseItem GetItem(int itemId)
    {

        foreach (BaseItem item in Items)
        {
            if (item.ItemID == itemId)
            {
                return item as BaseItem;
            }
        }
        return null;
    }
}
