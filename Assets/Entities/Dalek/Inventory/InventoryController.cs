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
    [SerializeField] private bool ForceUnlockAllWeapons = false;
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

    void Awake()
    {
        Unlocks = new WeaponUnlocks(true);
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

        StartCoroutine(UpdateAbilities());

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
        Player._playerComponent.CanSeeHiddenObjects = false;
        Player._playerComponent.SetPrivileges(false);
        Player._movement.CanElevate = false;

        //TODO add the rest of the conditions when the rest of the abilities are added
        //Namely checks for if the player is allowed to elevate, open doors, or see hidden enemies and stuff
    }


    /// <summary>
    /// Runs after equipping an item or closing the inventory UI
    /// Enables/Disables the various scripts governing behaviour based on which items are equipped or not
    /// </summary>
    private IEnumerator UpdateAbilities()
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
                    Player._playerComponent.SetPrivileges(true);
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
                    Player._movement.CanElevate = true;
                    break;
                case "Vision":
                    Player._playerComponent.CanSeeHiddenObjects = true;
                    break;
                case "Gattling":
                    Player._attackController.EnableGattlingGun = true;
                    break;
                case "Sneak":
                    //TODO Add Sneak behavior
                    break;
            }
        }
        SaveWeaponUnlocks(Unlocks);
        yield return null;
    }

    public void RunUpdateAbilities()
    {
        StartCoroutine(UpdateAbilities());
    }


    public void SaveWeaponUnlocks(WeaponUnlocks weaponUnlocks)
    {
        string json = JsonUtility.ToJson(weaponUnlocks);
        string jsonPath = Path.Combine(Application.persistentDataPath, "WeaponUnlocks.json");
        System.IO.File.WriteAllText(jsonPath, json);
    }


    public WeaponUnlocks LoadWeaponUnlocks()
    {

        if (ForceUnlockAllWeapons)
        {
            WeaponUnlocks unlocks = new WeaponUnlocks();
            unlocks.UnlockedItems = GameManager.GetItemDatabase().Items;
            return unlocks;
        }

        string jsonPath = Path.Combine(Application.persistentDataPath, "WeaponUnlocks.json");
        if (System.IO.File.Exists(jsonPath))
        {
            string json = System.IO.File.ReadAllText(jsonPath);
            WeaponUnlocks weaponUnlocks = JsonUtility.FromJson<WeaponUnlocks>(json);
            //This step is required to clean out any erroneous items which have wiggled their way into the file 
            weaponUnlocks.UnlockedItems.RemoveAll(a=>a == null);
            SaveWeaponUnlocks(weaponUnlocks);

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
            return new WeaponUnlocks(true);
        }
    }
}

[System.Serializable]
public class WeaponUnlocks
{
    [SerializeField] public List<Item> UnlockedItems;

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
