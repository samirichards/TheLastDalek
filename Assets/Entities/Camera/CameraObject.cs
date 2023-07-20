using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObject : MonoBehaviour
{
    GameObject TrackerTarget;
    private static CameraObject _Instance;
    [SerializeField] AudioClip LevelMusic;
    AudioSource MusicPlayer;

    private static GameObject CameraObjectReference;
    // Start is called before the first frame update

    public static CameraObject Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new CameraObject();
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
        _Instance = new CameraObject();
        CameraObjectReference = gameObject;
        DontDestroyOnLoad(CameraObjectReference);
    }
    void Start()
    {
        MusicPlayer = CameraObjectReference.AddComponent(typeof(AudioSource)) as AudioSource;
        TrackerTarget = Player.GetPlayerReference();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = TrackerTarget.transform.position;
        newPosition.y++;
        transform.position = newPosition;
        if (!MusicPlayer.isPlaying)
        {
            MusicPlayer.clip = LevelMusic;
            MusicPlayer.loop = true;
            MusicPlayer.Play();
        }
    }
}
