using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(menuName ="Artefact")]
[Serializable]
public class Item : ScriptableObject
{
    [SerializeField] public int ItemID;
    [SerializeField] public string ItemName;
    [SerializeField] public string ItemTitle;
    [SerializeField] public int ItemTotalTiers;
    [SerializeField] public int _itemTier;
    [SerializeField] public List<Sprite> ItemTextures;
    [SerializeField] public List<string> ItemDescription;
    [SerializeField] public List<Sprite> ItemModelTexture;
    [SerializeField] public List<GameObject> WorldModels;
    public bool isSelected = false;
    public Item()
    {
        _itemTier = 1;
    }

    public Item(Item item)
    {
        ItemName = item.ItemName;
        ItemID = item.ItemID;
        _itemTier = item._itemTier;
    }
}
