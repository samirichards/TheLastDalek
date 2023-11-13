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
        if (!GameObject.Find("Player"))
        {
            GameObject temp = Instantiate(PlayerPrefab);
            temp.transform.position = DefaultSpawnLocation;
            temp.GetComponent<Player>().cameraInstance.SetTracker();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {

    }
}
