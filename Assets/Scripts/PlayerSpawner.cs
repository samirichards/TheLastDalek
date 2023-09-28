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
        Player.Instance.transform.position = DefaultSpawnLocation;
    }
}
