using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Start is called before the first frame update
    public InventoryObject _InventoryObject;
    public ItemDatabaseObject ItemDatabase;
    public List<Item> EquippedItems = new List<Item>(2);
    public AudioClip ItemSelectionSound;
    public AudioClip ItemHoverSound;
    void Start()
    {
        UpdatePlayerAbilities();
    }

    void Awake()
    {
        EquippedItems = new List<Item>(2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<InventorySlot> GetItems()
    {
        foreach (var slot in _InventoryObject.Container.Items)
        {
            Debug.Log(slot.item.ItemName);
        }
        return _InventoryObject.Container.Items;
    }

    public void ClearInventory()
    {
        _InventoryObject.Container.Clear();
    }

    public bool AddItem(Item _item)
    {
        Debug.Log("Attempting to add an item");
        try
        {
            if (_InventoryObject.HasItem(_item.ItemTitle))
            {
                _InventoryObject.UpgradeItem(_item.ItemID);
                Debug.Log(_item.ItemTitle + " was upgraded");
                return _InventoryObject.HasItem(_item.ItemTitle);
            }
            else
            {
                _InventoryObject.AddItem(_item);
                Debug.Log(_item.ItemTitle + " was added to the inventory");
                //IInventory.Save();
                return _InventoryObject.HasItem(_item.ItemTitle);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    public void UpdatePlayerAbilities()
    {
        
    }
}
