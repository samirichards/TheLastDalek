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
    [SerializeField]private GameObject modelHolder;

    public void OnAfterDeserialize()
    {
    }

    public void Start()
    {
        _item._itemTier = ItemTier;
        //If the item is already in the players inventory then remove from the scene to prevent duplicates and stuff
        bool isDuplicate = Player._inventoryController.Unlocks.UnlockedItems.Any(a => a.ItemTitle == _item.ItemTitle && a._itemTier >= ItemTier);
        if (isDuplicate)
        {
            Destroy(gameObject);
        }
        if (_item.WorldModels != null && _item.WorldModels.Count > _item._itemTier && _item.WorldModels[_item._itemTier] != null)
        {
            // Instantiate the 3D model
            var position = transform.position;
            //position.y += 1;
            GameObject model = Instantiate(_item.WorldModels[_item._itemTier], position, Quaternion.identity, modelHolder.transform);
            model.name = $"{_item.ItemName}_Model_Tier{_item._itemTier}";
        }
        else
        {
            // Fallback to 2D sprite
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = true;
            Billboard billboard = GetComponent<Billboard>();
            billboard.enabled = true;
            if (spriteRenderer != null && _item.ItemModelTexture.Count > _item._itemTier)
            {
                spriteRenderer.sprite = _item.ItemModelTexture[_item._itemTier];
            }
        }
    }

    public void OnBeforeSerialize()
    {
        //_item._itemTier = ItemTier;
        //if (_item.WorldModels == null)
        //{
        //    GetComponentInChildren<SpriteRenderer>().sprite = _item.ItemModelTexture[_item._itemTier];
        //    //EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Destroy(gameObject);
        }
    }
}