using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] public AnimationCurve CameraShakeCurve;
    private bool isSet = false;
    [SerializeField] private Quaternion rotation;

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
        SetTracker();
    }

    public void SetTracker()
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
            isSet = (TrackerTarget != null);
        }

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        MusicPlayer = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        MusicPlayer.volume = GameSettings.GetSettings().MusicVolume;
        PlayMusicForScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        GameSettings.OnOptionsChanged += UpdateSettings;
        UpdateSettings(GameSettings.GetSettings());
        this.transform.rotation = rotation;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_Instance == null || this == null)
        {
            Debug.LogWarning("CameraObject instance is null or destroyed. Skipping OnSceneLoaded.");
            return;
        }

        // Call PlayMusicForScene safely
        PlayMusicForScene(scene.name);
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event when the object is destroyed.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        GameSettings.OnOptionsChanged -= UpdateSettings;
    }

    public void PlayMusicForScene(string sceneName)
    {
        // Ensure MusicPlayer is not null
        if (MusicPlayer == null)
        {
            Debug.LogWarning("MusicPlayer is null. Cannot play music for scene: " + sceneName);
            return;
        }

        foreach (var mapping in sceneMusicMapping.sceneMusicPairs)
        {
            if (mapping.sceneName == sceneName)
            {
                if (MusicPlayer.clip != mapping.musicClip)
                {
                    MusicPlayer.Stop();
                    MusicPlayer.clip = mapping.musicClip;
                    MusicPlayer.loop = true;
                    MusicPlayer.volume = MusicVolume;
                    MusicPlayer.Play();
                }
                return;
            }
        }

        Debug.LogWarning("Music track not found for scene: " + sceneName);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsStaticCamera && isSet)
        {
            Vector3 newPosition = TrackerTarget.transform.position;
            newPosition.y++;
            transform.position = newPosition;
        }
        if (!MusicPlayer.isPlaying && isSet)
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
