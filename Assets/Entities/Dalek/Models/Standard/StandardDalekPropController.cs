using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardDalekPropController : PropController
{

    [Header("Standard Specific Properties")]
    [SerializeField] private Material inactiveEmittersMaterial;
    [SerializeField] private Material activeEmittersMaterial;
    [SerializeField] private MeshRenderer emitters;
    protected override IEnumerator EmitterVOFlash(float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        SetEmittersActive(false);

        while (Time.time < endTime)
        {
            // Use the normalized time to vary the light intensity
            float amplitude = base.GetAudioAmplitude(); // Implement a method to get audio amplitude
            float intensity = Mathf.Lerp(0f, 1f, amplitude);

            if (intensity > 0.01f)
            {
                SetEmittersActive(true);
            }
            else
            {
                SetEmittersActive(false);
            }

            yield return null; // Wait for the next frame
        }

        SetEmittersActive(false);
    }

    public override void SetEmittersActive(bool state)
    {
        if (state)
        {
            emitters.materials = new Material[] { activeEmittersMaterial };
        }
        else
        {
            emitters.materials = new Material[] { inactiveEmittersMaterial };
        }
    }
}
