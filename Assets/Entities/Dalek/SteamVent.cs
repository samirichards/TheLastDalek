using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SteamVent : MonoBehaviour
{

    public VisualEffect SteamEffect;
    public float SteamDuration = 0.66f;
    // Start is called before the first frame update
    void Start()
    {
        SteamEffect.enabled = true;
        SteamEffect.Play();
        StartCoroutine(RunDuration(SteamDuration));
    }
    IEnumerator RunDuration(float time)
    {
        yield return new WaitForSeconds(time);
        SteamEffect.Stop();
        yield return new WaitForSeconds(3);
        Destroy(this);
    }
}
