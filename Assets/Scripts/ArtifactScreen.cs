using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArtifactScreen : MonoBehaviour
{
    public GameObject[] InventorySlots;
    public GameObject SelectedSlotA;
    public GameObject SelectedSlotB;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopulateSlots()
    {
        for (int i = 0; i < Player._inventoryController.Unlocks.UnlockedItems.Count; i++)
        {
            InventorySlots[i].GetComponent<ItemSlot>().SetDisplayedImage(Player._inventoryController.Unlocks
                .UnlockedItems[i].ItemTextures[Player._inventoryController.Unlocks.UnlockedItems[i]._itemTier]);
        }
    }
}
