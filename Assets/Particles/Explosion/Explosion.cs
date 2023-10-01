using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Explosion : MonoBehaviour
{
    [SerializeField] private AudioClip[] explosionSounds;
    private AudioSource src;

    // Start is called before the first frame update
    void Start()
    {
        src.PlayOneShot(explosionSounds[Random.Range(0, explosionSounds.Length)]);
        StartCoroutine(DespawnTimer(6));
    }

    void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DespawnTimer(float duration)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
    }
}
