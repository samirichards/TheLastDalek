using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Mainmenu : MonoBehaviour
{
    [SerializeField] UIDocument _uiDocument = null;

    void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;
        root.Query<UnityEngine.UIElements.Button>("Main_Btn_Start").First().clicked += StartButton_clicked;
        root.Query<UnityEngine.UIElements.Button>("Main_Btn_Options").First().clicked += OptionsButton_Clicked;
        root.Query<UnityEngine.UIElements.Button>("Main_Btn_Extras").First().clicked += ExtrasButton_Clicked;
        root.Query<UnityEngine.UIElements.Button>("Main_Btn_Quit").First().clicked += QuitButton_Clicked;
    }

    private void QuitButton_Clicked()
    {
        Application.Quit(0);
    }

    private void ExtrasButton_Clicked()
    {
        Debug.Log("Extras Button Clicked");
    }

    private void OptionsButton_Clicked()
    {
        Debug.Log("Options Button Clicked");
    }

    private void StartButton_clicked()
    {
        SceneManager.LoadScene("DevScene");
    }

    public void StartLevel(int levelID)
    {

    }

    public void LoadDevLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }
    
    public void QuitGame()
    {

    }
}
