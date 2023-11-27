using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraObject : MonoBehaviour
{
    public GameObject TrackerTarget;
    public static CameraObject _Instance;
    [SerializeField] AudioClip LevelMusic;
    AudioSource MusicPlayer;
    [SerializeField]public SceneMusicMapping sceneMusicMapping;
    [SerializeField] public float MusicVolume = 0.75f;
    [SerializeField] public bool IsStaticCamera = false;

    public static GameObject CameraObjectReference;
    // Start is called before the first frame update

    public static CameraObject Instance
    {
        get
        {
            if (!_Instance)
            {
                //_Instance = new CameraObject();
                CameraObjectReference = GameObject.Find("CameraObject");
                _Instance = CameraObjectReference.GetComponent<CameraObject>();
                _Instance.name = _Instance.GetType().ToString();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(CameraObjectReference);
            }
            return _Instance;
        }
    }

    void Awake()
    {

    }
    void Start()
    {
        if (!IsStaticCamera)
        {
            if (_Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                _Instance = this;
            }
            else if (_Instance != this)
            {
                Destroy(gameObject);
            }
            CameraObjectReference = gameObject;
            DontDestroyOnLoad(CameraObjectReference);
            TrackerTarget = Player.GetPlayerReference();
        }

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        MusicPlayer = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        MusicPlayer.volume = GameSettings.GetSettings().MusicVolume;
        PlayMusicForScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        GameSettings.OnOptionsChanged += UpdateSettings;
        UpdateSettings(GameSettings.GetSettings());
    }

    public void SetTracker()
    {
        TrackerTarget = Player.GetPlayerReference();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This method will be called whenever a new scene is loaded.
        // You can call your UpdateMusic function here.
        PlayMusicForScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event when the object is destroyed.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlayMusicForScene(string sceneName)
    {
        foreach (var mapping in sceneMusicMapping.sceneMusicPairs)
        {
            if (mapping.sceneName == sceneName)
            {
                if (MusicPlayer.clip != mapping.musicClip)
                {
                    //Continue the music if the new clip is the same clip (to stop the music stopping and starting as the player changes room)
                    MusicPlayer.Stop();
                    MusicPlayer.clip = mapping.musicClip;
                    MusicPlayer.loop = true;
                    MusicPlayer.volume = MusicVolume;
                    MusicPlayer.Play();
                }
                return; // Exit the loop once a match is found
            }
        }

        Debug.LogWarning("Music track not found for scene: " + sceneName);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsStaticCamera)
        {
            Vector3 newPosition = TrackerTarget.transform.position;
            newPosition.y++;
            transform.position = newPosition;
        }
        if (!MusicPlayer.isPlaying)
        {
            PlayMusicForScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    public void UpdateSettings(Options options)
    {
        MusicVolume = options.MusicVolume;
        MusicPlayer.volume = MusicVolume;
    }

    void OnApplicationQuit()
    {
        GameSettings.OnOptionsChanged -= UpdateSettings;
    }
}
