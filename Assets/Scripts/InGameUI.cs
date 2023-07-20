using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    private static InGameUI _inGameUiInstance;
    private static GameObject UICanvasRef;

    [SerializeField] public static GameObject PauseMenu;
    [SerializeField] public static GameObject ItemUpgradeScreen;
    [SerializeField] public static GameObject ArtifactScreen;

    // Start is called before the first frame update

    public static InGameUI Instance
    {
        get
        {
            if (!_inGameUiInstance)
            {
                _inGameUiInstance = new InGameUI();
                UICanvasRef = GameObject.FindWithTag("InGameUI");
                _inGameUiInstance = UICanvasRef.GetComponent<InGameUI>();
                _inGameUiInstance.name = _inGameUiInstance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(UICanvasRef);
            }
            return _inGameUiInstance;
        }
    }

    public static GameObject GetUICanvasRef()
    {
        return UICanvasRef;
    }

    void Awake()
    {
        UICanvasRef = GameObject.FindWithTag("InGameUI");
        PauseMenu = GameObject.FindWithTag("PauseMenu");
        ItemUpgradeScreen = GameObject.FindWithTag("ItemUpgradeScreen");
        ArtifactScreen = GameObject.FindWithTag("ArtifactScreen");
        DontDestroyOnLoad(UICanvasRef);
    }

    void Start()
    {

    }
}
