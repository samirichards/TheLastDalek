using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] public WeaponUnlocks Unlocks;

    [SerializeField] private List<Item> equippedItems;
    public List<Item> EquippedItems
    {
        get
        {
            return equippedItems;
        }

        set
        {
            equippedItems = value;
            UpdateAbilities();
        }
    }



    /// <summary>
    /// This is just so that I can equip items from the editor and it'll actually update
    /// Equipping items through the editor allows me to also equip more than 2 items, which is pretty fun
    /// </summary>
    private void OnValidate()
    {
        EquippedItems = equippedItems;
    }


    // Start is called before the first frame update
    void Start()
    {
        Unlocks = LoadWeaponUnlocks();
        EquippedItems = new List<Item>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    void OnCollisionEnter(Collision collision)
    {
        var item = collision.gameObject.GetComponent<GroundItem>();
        if (item)
        {
            AddItemToInventory(item._item);
            Destroy(item.gameObject);
        }
    }


    void AddItemToInventory(Item _item)
    {
        if (_item == null)
        {
            Debug.Log("_item was null for some reason");
            return;
        }

        if (Unlocks.UnlockedItems.FirstOrDefault(a => a.ItemTitle == _item.ItemTitle) != null)
        {
            // If an item with the same title exists, just upgrade its item tier
            Unlocks.UnlockedItems.First(a => a.ItemTitle == _item.ItemTitle)._itemTier++;
            Debug.Log("Inventory already contains " + _item.ItemTitle + " so just upgrade its tier to " + Unlocks.UnlockedItems.First(a => a.ItemTitle == _item.ItemTitle)._itemTier);
        }
        else
        {
            Unlocks.UnlockedItems.Add(_item);
            Debug.Log("Inventory did not contain " + _item.ItemTitle + ", so it has been added");
        }

        SaveWeaponUnlocks(Unlocks);
        GameManager.ShowUpgradeScreen(_item.ItemID, _item._itemTier);
    }


    public void EquipItem(int itemID)
    {
        Item itemToEquip = Unlocks.UnlockedItems.FirstOrDefault(item => item.ItemID == itemID);
        if (itemToEquip != null)
        {
            EquipItem(itemToEquip);
        }
        else
        {
            Debug.Log("Item with ID " + itemID + " not found.");
        }
    }


    public void EquipItem(Item item)
    {
        if (EquippedItems.Count < 2)
        {
            EquippedItems.Add(item);
            Debug.Log(item.ItemTitle + " has been equipped.");
        }
        else
        {
            // If already 2 items equipped, de-equip the first one and equip the new one.
            Item firstEquippedItem = EquippedItems[0];
            EquippedItems.RemoveAt(0);
            EquippedItems.Add(item);
            Debug.Log(firstEquippedItem.ItemTitle + " has been unequipped.");
            Debug.Log(item.ItemTitle + " has been equipped.");
        }
        UpdateAbilities();
    }


    public void ClearAllAbilities()
    {
        Player._movement.IsMovementEnhanced = false;
        Player._chestRotateController.IsRotationAllowed = false;
        Player._attackController.GunStickEnabled = false;
        Player._attackController.LaserType = 0;
        Player._attackController.EnableGattlingGun = false;
        Player.GetShieldManagerReference().ShieldSetInactive();
        Player.GetShieldManagerReference().ShieldTier = 0;

        //TODO add the rest of the conditions when the rest of the abilities are added
        //Namely checks for if the player is allowed to elevate, open doors, or see hidden enemies and stuff
    }


    /// <summary>
    /// Runs after equipping an item or closing the inventory UI
    /// Enables/Disables the various scripts governing behaviour based on which items are equipped or not
    /// </summary>
    public void UpdateAbilities()
    {
        ClearAllAbilities();
        foreach (Item equippedItem in EquippedItems)
        {
            //This isn't pretty, but tbh it doesn't really matter, if you can think of a better way to do it then go for it but it really won't improve anything
            //Ig doing it this way makes it harder for modders to add new items but like... That's not my problem lmao
            //Wait no, there would be a way to describe on the items themselves which scripts they turn on or off with which parameters, but I cba doing that, let's stick with the pragmatic approach
            switch (equippedItem.ItemTitle)
            {
                case "Gun":
                    Player._attackController.LaserType = (uint)equippedItem._itemTier;
                    Player._attackController.GunStickEnabled = true;
                    break;
                case "Processor":
                    //TODO Add Processor behavior
                    break;
                case "Shield":
                    Player.GetShieldManagerReference().ShieldTier = equippedItem._itemTier;
                    Player.GetShieldManagerReference().ShieldSetActive();
                    break;
                case "SpeedUp":
                    Player._movement.IsMovementEnhanced = true;
                    break;
                case "Gears":
                    Player._chestRotateController.IsRotationAllowed = true;
                    break;
                case "Elevate":
                    //TODO Add Elevate behavior
                    break;
                case "Vision":
                    //TODO Add Vision behavior
                    break;
                case "Gattling":
                    Player._attackController.EnableGattlingGun = true;
                    break;
                case "Sneak":
                    //TODO Add Sneak behavior
                    break;
            }
        }
    }


    public void SaveWeaponUnlocks(WeaponUnlocks weaponUnlocks)
    {
        string json = JsonUtility.ToJson(weaponUnlocks);
        System.IO.File.WriteAllText(Path.Combine(Application.dataPath, "WeaponUnlocks.json"), json);
    }


    public WeaponUnlocks LoadWeaponUnlocks()
    {
        if (System.IO.File.Exists(Path.Combine(Application.dataPath, "WeaponUnlocks.json")))
        {
            string json = System.IO.File.ReadAllText(Path.Combine(Application.dataPath, "WeaponUnlocks.json"));
            WeaponUnlocks weaponUnlocks = JsonUtility.FromJson<WeaponUnlocks>(json);

            // Check if UnlockedItems is null and initialize it if needed
            if (weaponUnlocks.UnlockedItems == null)
            {
                weaponUnlocks.UnlockedItems = new List<Item>();
            }

            return weaponUnlocks;
        }
        else
        {
            // If the file doesn't exist, return a new WeaponUnlocks object with UnlockedItems initialized
            return new WeaponUnlocks { UnlockedItems = new List<Item>() };
        }
    }
}

[System.Serializable]
public class WeaponUnlocks
{
    public List<Item> UnlockedItems { get; set; }

    public WeaponUnlocks()
    {

    }


    /// <summary>
    /// If bool is set (doesn't really matter what value you use) then return WeaponUnlocks with a fresh UnlockedItems list
    /// This is used when unity can't find an existing Json file for the weapon unlocks
    /// </summary>
    /// <param name="useEmpty"></param>
    public WeaponUnlocks(bool useEmpty)
    {
        UnlockedItems = new List<Item>();
    }
}
