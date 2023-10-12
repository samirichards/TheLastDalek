using System.Collections;
using UnityEngine;

public class GunStickAimController : MonoBehaviour
{
    [SerializeField] public bool IsSwivelAllowed = true;
    [SerializeField] private GameObject GunStick;
    [SerializeField] private GameObject BodyBase;
    [SerializeField] private float maxRotationAngle = 20f;
    [SerializeField] private float Offset = 180;
    private Quaternion initialPosition;

    private bool isFiring = false;
    private float fireTime = 0;

    void Awake()
    {
        initialPosition = GunStick.transform.localRotation;
    }
    //TODO just fucking redo all of this, I asked chatgpt to help and tbh it's wasted more time than if I just did it myself lol

    // Call this method to aim the gunstick toward a target with a specific duration and cooldown time
    public void AimGunstickTowards(Vector3 target, float duration, float cooldown)
    {
        if (!IsSwivelAllowed || isFiring)
            return;

        // Gradually interpolate the rotation over the specified duration
        Quaternion initialRotation = initialPosition;
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - target, transform.up);
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y + Offset, targetRotation.eulerAngles.z);
        targetRotation = LimitRotation(targetRotation, maxRotationAngle);

        Debug.Log("Initial Rotation: " + initialRotation.eulerAngles);
        Debug.Log("Target Rotation: " + targetRotation.eulerAngles);

        StartCoroutine(AimGunstickCoroutine(initialRotation, targetRotation, duration, cooldown));
    }

    private IEnumerator AimGunstickCoroutine(Quaternion startRotation, Quaternion targetRotation, float duration, float cooldown)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            GunStick.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Mark as firing and set cooldown time
        isFiring = true;
        GetComponent<AttackController>().HandleGunStick();
        fireTime = Time.time;

        // Wait for the cooldown before resetting the gunstick
        yield return new WaitForSeconds(cooldown);

        // Reset gunstick to its default position
        isFiring = false;
        GunStick.transform.localRotation = initialPosition;
    }

    private Quaternion LimitRotation(Quaternion rotation, float maxAngle)
    {
        Quaternion limitedRotation = rotation;
        float angle = Quaternion.Angle(transform.localRotation, rotation);
        if (angle > maxAngle)
        {
            Quaternion maxRotation = Quaternion.RotateTowards(transform.localRotation, rotation, maxAngle);
            limitedRotation = Quaternion.RotateTowards(maxRotation, rotation, -maxAngle);
        }
        return limitedRotation;
    }
}