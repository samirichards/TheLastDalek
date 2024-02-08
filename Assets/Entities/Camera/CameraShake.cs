using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private AnimationCurve shakeCurve;

    public void StartCameraShake(float duration, float intensity)
    {
        StartCoroutine(Shaking(duration, intensity));
    }

    private IEnumerator Shaking(float duration, float intensity)
    {
        Vector3 initialPos = transform.localPosition;
        float timer = 0f;
        while (timer < duration)
        {
            Vector3 shakePosition = initialPos;
            float currentIntensity = (intensity * shakeCurve.Evaluate(timer / duration));
            shakePosition.x = Random.Range(initialPos.x - currentIntensity * 2.5f, initialPos.x + currentIntensity * 2.5f);
            shakePosition.y = Random.Range(initialPos.y - currentIntensity, initialPos.y + currentIntensity);
            shakePosition.z = Random.Range(initialPos.z - currentIntensity, initialPos.z + currentIntensity);
            transform.SetLocalPositionAndRotation(shakePosition, Quaternion.identity);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = initialPos;
    }
}
