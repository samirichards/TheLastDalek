using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public Item _item;
    public int ItemTier;

    public void OnAfterDeserialize()
    {
    }

    public void Start()
    {
        _item._itemTier = ItemTier;
        //If the item is already in the players inventory then remove from the scene to prevent duplicates and stuff
        bool temp = Player._inventoryController.Unlocks.UnlockedItems.Any(a => a.ItemTitle == _item.ItemTitle && a._itemTier >= ItemTier);
        if (temp)
        {
            Destroy(gameObject);
        }
    }

    public void OnBeforeSerialize()
    {
        _item._itemTier = ItemTier;
        GetComponentInChildren<SpriteRenderer>().sprite = _item.ItemModelTexture[_item._itemTier];
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Destroy(gameObject);
        }
    }
}