using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]

public class EnergyDischargeCollisionController : MonoBehaviour
{
    private Light emitter;
    [SerializeField] private float FadeTime = 2.0f;
    public Color L1_Color;
    public Color L2_Color;
    public Color L3_Color;
    [SerializeField]private ParticleSystem m_ParticleSystem;
    void Start()
    {
        emitter = GetComponent<Light>();
        StartCoroutine(fadeInAndOut(emitter, false, FadeTime));
        m_ParticleSystem.Play();
    }

    IEnumerator fadeInAndOut(Light lightToFade, bool fadeIn, float duration)
    {
        float minLuminosity = 0f; // min intensity
        float maxLuminosity = 0.1f; // max intensity

        float counter = 0f;

        //Set Values depending on if fadeIn or fadeOut
        float a, b;

        if (fadeIn)
        {
            a = minLuminosity;
            b = maxLuminosity;
        }
        else
        {
            a = maxLuminosity;
            b = minLuminosity;
        }

        float currentIntensity = lightToFade.intensity;

        while (counter < duration)
        {
            counter += Time.deltaTime;

            lightToFade.intensity = Mathf.Lerp(a, b, counter / duration);

            yield return null;
        }

        if (counter > duration)
        {
            Destroy(gameObject);
        }
    }

    public void SetLightType(uint explosionType)
    {
        switch (explosionType)
        {
            default:
                GetComponent<Light>().color = L1_Color;
                break;
            case 0:
                GetComponent<Light>().color = L1_Color;
                break; 
            case 1:
                GetComponent<Light>().color = L2_Color;
                break; 
            case 2:
                GetComponent<Light>().color = L3_Color;
                break;

        }
    }
}
