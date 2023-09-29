using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraObject : MonoBehaviour
{
    GameObject TrackerTarget;
    private static CameraObject _Instance;
    [SerializeField] AudioClip LevelMusic;
    AudioSource MusicPlayer;
    [SerializeField]public SceneMusicMapping sceneMusicMapping;
    [SerializeField] public float MusicVolume = 0.75f;

    private static GameObject CameraObjectReference;
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Start()
    {
        MusicPlayer = CameraObjectReference.AddComponent(typeof(AudioSource)) as AudioSource;
        MusicPlayer.volume = MusicVolume;
        TrackerTarget = Player.GetPlayerReference();
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This method will be called whenever a new scene is loaded.
        // You can call your UpdateMusic function here.
        UpdateMusic();
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event when the object is destroyed.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlayMusicForScene(string sceneName)
    {
        foreach (var mapping in sceneMusicMapping.sceneMusicPairs)
        {
            if (mapping.sceneName == sceneName)
            {
                MusicPlayer.clip = mapping.musicClip;
                MusicPlayer.loop = true;
                MusicPlayer.volume = MusicVolume;
                MusicPlayer.Play();
                return; // Exit the loop once a match is found
            }
        }

        Debug.LogWarning("Music track not found for scene: " + sceneName);
    }

    public void UpdateMusic()
    {
        MusicPlayer.Stop();
        MusicPlayer.volume = MusicVolume;
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = TrackerTarget.transform.position;
        newPosition.y++;
        transform.position = newPosition;
        if (!MusicPlayer.isPlaying)
        {
            PlayMusicForScene(SceneManager.GetActiveScene().name);
        }
    }
}
