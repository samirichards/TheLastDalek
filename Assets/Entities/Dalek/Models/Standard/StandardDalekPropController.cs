using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardDalekPropController : PropController
{
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
}
