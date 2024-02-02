using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] public Vector3 DefaultSpawnLocation;
    [SerializeField] public GameObject PlayerPrefab;

    [SerializeField] public GameObject CameraPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        if (!GameObject.Find("Player"))
        {
            GameObject temp = Instantiate(PlayerPrefab, DefaultSpawnLocation, Quaternion.identity);
            DontDestroyOnLoad(temp);
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
            OnDalekSpawned?.Invoke(this, new PlayerSpawnedArgs(temp));
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
