using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] public Vector3 DefaultSpawnLocation;
    [SerializeField] public GameObject PlayerPrefab;

    [SerializeField] public GameObject CameraPrefab;
    GameObject playerInstance;
    bool playerSpawned = false;
    // Start is called before the first frame update
    void Start()
    {
        if (playerSpawned && OnDalekSpawned != null)
        {
            OnDalekSpawned.Invoke(this, new PlayerSpawnedArgs(playerInstance));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        if (!GameObject.Find("Player"))
        {
            playerInstance = Instantiate(PlayerPrefab, DefaultSpawnLocation, Quaternion.identity);
            DontDestroyOnLoad(playerInstance);
            if (GameObject.FindGameObjectWithTag("CameraObject") == null)
            {
                var cameraObject = Instantiate(CameraPrefab, transform.position, transform.rotation);
                DontDestroyOnLoad(cameraObject);
                cameraObject.GetComponent<CameraObject>().SetTracker();
            }
            else
            {
                //temp.GetComponent<Player>().cameraInstance.SetTracker();
            }
            playerSpawned = (playerInstance != null);
        }
    }

    public static event EventHandler<PlayerSpawnedArgs> OnDalekSpawned;

    public static EventHandler DalekSpawned;
}

public class PlayerSpawnedArgs : EventArgs
{
    public GameObject player;

    public PlayerSpawnedArgs(GameObject _player)
    {
        player = _player;
    }
}
