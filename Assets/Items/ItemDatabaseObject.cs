using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public List<Item> Items;

    public void OnAfterDeserialize()
    {

    }

    public void OnBeforeSerialize()
    {

    }
}