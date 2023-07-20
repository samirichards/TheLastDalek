using System.Collections;
using System.Collections.Generic;
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