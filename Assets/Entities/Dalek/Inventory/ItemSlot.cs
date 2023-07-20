using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ItemSlot : MonoBehaviour
{
    [SerializeField]private GameObject ImageSlot;
    [SerializeField] private GameObject SelectionGraphic;
    [SerializeField] private Sprite EmptySlot;
    [SerializeField] private Sprite OccupiedSlot;
    public void SetDisplayedImage(Sprite sprite)
    {
        GetComponent<Image>().sprite = OccupiedSlot;
        ImageSlot.GetComponent<Image>().sprite = sprite;
        ImageSlot.GetComponent<Image>().enabled = true;
    }

    public void ClearDisplayedImage()
    {
        GetComponent<Image>().sprite = EmptySlot;
        ImageSlot.GetComponent<Image>().sprite = null;
        ImageSlot.GetComponent<Image>().enabled = false;
    }

    public void ShowSelectionGraphic()
    {
        SelectionGraphic.SetActive(true);
    }

    public void HideSelecionGraphic()
    {
        SelectionGraphic.SetActive(false);
    }
}
