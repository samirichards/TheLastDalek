using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;
using System.Linq;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public ItemDatabaseObject database;
    public Inventory Container;

    void Awake()
    {
        Container = new Inventory();
    }


    public void AddItem(Item _item)
    {
        if (HasItem(_item.ItemTitle))
        {
            UpgradeItem(_item.ItemID);
        }
        else
        {
            InventorySlot newSlot = new InventorySlot(_item);
            Container.Items.Add(newSlot);
        }

    }

    public void UpgradeItem(int itemID)
    {
        foreach (var item in Container.Items)
        {
            if (item.item.ItemID == itemID)
            {
                item.item._itemTier++;
                item.ItemTier = item.item._itemTier;
            }
        }
    }

    /// <summary>
    /// Method to find if inventory already contains the item or not (and if so, if the item is of a different tier)
    /// </summary>
    /// <param name="_item"></param>
    /// <returns>
    /// 0 = No item found
    /// 1 = Item found but lower tier
    /// 2 = Item found but higher tier
    /// -1 = Item found at same tier
    /// </returns>
    public bool HasItem(string itemTitle)
    {
        if (Container.Items.Count(a => a.item != null) > 0)
        {
            return Container.Items.Select(a => a.item.ItemTitle).Contains(itemTitle);
        }
        return false;
    }

    [ContextMenu("Save")]
    public void Save()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }
}
[System.Serializable]
public class Inventory
{
    public List<InventorySlot> Items;

    public Inventory()
    {
        Items = new List<InventorySlot>(9);
    }
    public void Clear()
    {
        Items.Clear();
    }
}
[System.Serializable]
public class InventorySlot
{
    public int ItemTier;
    public Item item;
    public InventorySlot()
    {
        item = null;
        ItemTier = 1;
    }
    public InventorySlot(Item _item)
    {
        item = _item;
        ItemTier = _item._itemTier;
    }
    public void UpdateSlot(Item _item)
    {
        item = _item;
        ItemTier = _item._itemTier;
    }
}