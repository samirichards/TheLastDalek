using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ItemSlot : MonoBehaviour
{
    [SerializeField]private GameObject ImageSlot;
    [SerializeField] private GameObject SelectionGraphic;
    [SerializeField] private Sprite EmptySlot;
    [SerializeField] private Sprite OccupiedSlot;
    public bool IsOccupied = false;

    void Awake()
    {
        ClearDisplayedImage();
    }

    public void SetDisplayedImage(Sprite sprite)
    {
        GetComponent<Image>().sprite = OccupiedSlot;
        ImageSlot.GetComponent<Image>().sprite = sprite;
        ImageSlot.GetComponent<Image>().enabled = true;
        IsOccupied = true;
    }

    public void ClearDisplayedImage()
    {
        GetComponent<Image>().sprite = EmptySlot;
        ImageSlot.GetComponent<Image>().sprite = null;
        ImageSlot.GetComponent<Image>().enabled = false;
        IsOccupied = false;
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
