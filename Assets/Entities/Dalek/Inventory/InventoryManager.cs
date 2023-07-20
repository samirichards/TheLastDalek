using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] InventorySlots;
    public GameObject SelectedSlotA;
    public GameObject SelectedSlotB;
    public GameObject ItemModelDisplay;
    public InventoryObject IInventory;
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
        InventorySlots = InGameUI.ArtifactScreen.GetComponent<ArtifactScreen>().InventorySlots;
        SelectedSlotA = InGameUI.ArtifactScreen.GetComponent<ArtifactScreen>().SelectedSlotA;
        SelectedSlotB = InGameUI.ArtifactScreen.GetComponent<ArtifactScreen>().SelectedSlotB;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawInventory()
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            var slot = InventorySlots[i].GetComponent<ItemSlot>();
            if (IInventory.Container.Items.Count > i)
            {
                if (IInventory.Container.Items[i].item)
                {
                    var currentItem = ItemDatabase.Items.First(a => a.ItemName == IInventory.Container.Items[i].item.ItemName);
                    Debug.Log("Slot " + i + " being drawn with " + IInventory.Container.Items[i].item.ItemName);
                    slot.SetDisplayedImage(currentItem.ItemTextures[IInventory.Container.Items[i].ItemTier]);
                }
                else
                {
                    slot.ClearDisplayedImage();
                }
            }
            else
            {
                slot.ClearDisplayedImage();
            }
        }
    }

    public void InventoryItemHovered(int slotNumber)
    {

    }

    public void InventoryItemHoverLeft(int slotNumber)
    {

    }

    public void InventorySlotClicked(int slotNumber)
    {
        if (IInventory.Container.Items[slotNumber] != null)
        {
            Debug.Log("Slot " + slotNumber + " has an " + IInventory.Container.Items[slotNumber].item.ItemName + " tier " + IInventory.Container.Items[slotNumber].item._itemTier + " in it");
            if (!EquippedItems.Contains(IInventory.Container.Items[slotNumber].item))
            {
                if (EquippedItems.Count == 2)
                {
                    EquippedItems.RemoveAt(0);
                    EquippedItems.Add(IInventory.Container.Items[slotNumber].item);
                }
                else
                {
                    EquippedItems.Add(IInventory.Container.Items[slotNumber].item);
                }
                GetComponent<AudioSource>().PlayOneShot(ItemSelectionSound);
            }
        }

        if (EquippedItems.Count > 0)
        {
            SelectedSlotA.GetComponent<ItemSlot>().SetDisplayedImage(EquippedItems[0].ItemTextures[EquippedItems[0]._itemTier]);
            SelectedSlotB.GetComponent<ItemSlot>().SetDisplayedImage(EquippedItems[1].ItemTextures[EquippedItems[1]._itemTier]);
        }
        else
        {
            SelectedSlotA.GetComponent<ItemSlot>().ClearDisplayedImage();
            SelectedSlotB.GetComponent<ItemSlot>().ClearDisplayedImage();
        }
    }

    public void SelectedItemSlotClicked(int slotNumber)
    {

    }

    public void UpdatePlayerAbilities()
    {
        //There is an elegant way to do this, and an easy way
        //This is the easy no thinky way
        //If this ends up on r/badcode idc, I'm too tired rn to make the elegant solution

        //Standard GunStick stats set
        if (EquippedItems.Any(a=>a.ItemTitle == "Gun"))
        {
            GetComponent<AttackController>().GunStickEnabled = true;
            GetComponent<AttackController>().LaserType = (uint)EquippedItems.First(a => a.ItemTitle == "Gun")._itemTier;
        }
        else
        {
            GetComponent<AttackController>().GunStickEnabled = false;
        }

        //Gattling gun Stats set
        //Doesn't just enable based off of expression since I might want to change the balance of the item later
        //(such as make the gattling gun fire the laser but at further reduced damage when the main gun isn't equipped)
        if (EquippedItems.Any(a=>a.ItemTitle == "Gattling"))
        {
            GetComponent<AttackController>().EnableGattlingGun = true;
        }
        else
        {
            GetComponent<AttackController>().EnableGattlingGun = false;
        }
        //Gears setter
        //This is pretty simple tbh
        GetComponent<ChestRotateController>().IsRotationAllowed = EquippedItems.Any(a => a.ItemTitle == "Gears");

        GetComponent<Movement>().IsMovementEnhanced = EquippedItems.Any(a => a.ItemTitle == "SpeedUp");

        if (EquippedItems.Any(a => a.ItemTitle == "Shield"))
        {
            GetComponentInChildren<ShieldManager>().ShieldEnabled = true;
            if (EquippedItems.Find(a=>a.ItemTitle == "Shield")._itemTier == 1)
            {
                GetComponentInChildren<ShieldManager>().ShieldMaxHealth = 200f;
                GetComponentInChildren<ShieldManager>().ShieldRechargeRate = 15f;
                GetComponentInChildren<ShieldManager>().ShieldRechargeDelay = 2f;
                gameObject.GetComponent<PlayerComponent>().shieldBarSlider.maxValue = 2f;
            }
            else
            {
                GetComponentInChildren<ShieldManager>().ShieldMaxHealth = 100f;
                GetComponentInChildren<ShieldManager>().ShieldRechargeRate = 6.666f;
                GetComponentInChildren<ShieldManager>().ShieldRechargeDelay = 3f;
                gameObject.GetComponent<PlayerComponent>().shieldBarSlider.maxValue = 1f;
            }
        }
        else
        {
            GetComponentInChildren<ShieldManager>().ShieldEnabled = false;
        }

    }
}
