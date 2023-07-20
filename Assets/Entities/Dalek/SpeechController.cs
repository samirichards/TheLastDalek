using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechController : MonoBehaviour
{
    [SerializeField] public Light LeftLightCluster;
    [SerializeField] public Light RightLightCluster;
    [SerializeField] public AudioSource SpeechAudioSource;
    [SerializeField] public bool ClustersEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        SpeechAudioSource.spatialBlend = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        LeftLightCluster.enabled = ClustersEnabled;
        RightLightCluster.enabled = ClustersEnabled;

        if (!GameManager.IsGamePaused)
        {
            ClustersEnabled = SpeechAudioSource.isPlaying;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!SpeechAudioSource.isPlaying)
                {
                    SpeechAudioSource.Play();
                }
            }
        }
    }
}
