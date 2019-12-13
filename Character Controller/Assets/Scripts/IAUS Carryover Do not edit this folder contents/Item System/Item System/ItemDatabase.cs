using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem;

static public class ItemDatabase
{
    /// The List of all the RPGItem assets from our resources
    static public List<BaseItem> _items;


    static private bool _isLoaded = false;
    /// <summary>
    /// Gets if the ItemDatabase loaded the RPGItem assets
    /// </summary>
    static public bool IsLoaded
    {
        get { return _isLoaded; }
    }

    /// <summary>
    /// Checks that our database is initialized
    /// </summary>
    static private void ValidateDatabase()
    {
        if (_items == null) _items = new List<BaseItem>();
    }

    /// <summary>
    /// Loads the RPGAssets from the database.
    /// Only loads if the database is not already loaded
    /// </summary>
    static public void LoadDatabase()
    {
        if (_isLoaded) return;
        LoadDatabaseForce();
    }

    /// <summary>
    /// Force Loads the RPGItem assets from the database.
    /// This does not check if the Database was previously
    /// Loaded and does not clear the database
    /// </summary>
    static public void LoadDatabaseForce()
    {
        ValidateDatabase();
        _isLoaded = true;
        BaseItem[] resources = Resources.LoadAll<BaseItem>(@"Items");
        foreach (BaseItem item in resources)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
            }
        }
    }

    /// <summary>
    /// Removes all RPGItem assets from the database
    /// </summary>
    static public void ClearDatabase()
    {
        _isLoaded = false;
        _items.Clear();
    }

    /// <summary>
    /// Returns a new instance of the RPGItem with
    /// the corrisponding itemID
    /// </summary>
    static public BaseItem GetItem(int itemId)
    {
        ValidateDatabase();
        LoadDatabase();
        foreach (BaseItem item in _items)
        {
            if (item.ItemID == itemId)
            {
                return ScriptableObject.Instantiate(item) as BaseItem;
            }
        }
        return null;
    }
}
