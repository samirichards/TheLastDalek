using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        root.Query<UnityEngine.UIElements.Button>("Btn_Start_NewGame").First().clicked += Start_NewGameButton_Clicked;
        root.Query<UnityEngine.UIElements.Button>("Btn_Start_LoadGame").First().clicked += Start_LoadGameButton_Clicked;
        root.Query<UnityEngine.UIElements.Button>("Btn_Start_DevScene").First().clicked += Start_DevSceneButton_Clicked;
        root.Query<UnityEngine.UIElements.Button>("Btn_Start_Back").First().clicked += Start_BackButton_Clicked;
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
        _uiDocument.rootVisualElement.Query<UnityEngine.UIElements.GroupBox>("Main").First().style.display =
            DisplayStyle.None;
            _uiDocument.rootVisualElement.Query<UnityEngine.UIElements.GroupBox>("StartOptions").First().style.display =
                DisplayStyle.Flex;
    }

    private void Start_NewGameButton_Clicked()
    {
        WipeData();
        UnityEngine.SceneManagement.SceneManager.LoadScene("F1_R0");
    }

    private void Start_LoadGameButton_Clicked()
    {

    }

    private void Start_DevSceneButton_Clicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DevScene");
    }

    private void WipeData()
    {
        string jsonPath = Path.Combine(Application.persistentDataPath, "WeaponUnlocks.json");
        File.Delete(jsonPath);
        //TODO delete progress path as well when you make it
    }

    private void Start_BackButton_Clicked()
    {
        _uiDocument.rootVisualElement.Query<UnityEngine.UIElements.GroupBox>("Main").First().style.display =
            DisplayStyle.Flex;
        _uiDocument.rootVisualElement.Query<UnityEngine.UIElements.GroupBox>("StartOptions").First().style.display =
            DisplayStyle.None;
    }

    public void StartLevel(int levelID)
    {

    }

    public void QuitGame()
    {

    }
}
