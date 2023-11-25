using System.Collections;
using UnityEngine;

public class GunStickAimController : MonoBehaviour
{
    [SerializeField] public bool IsSwivelAllowed = true;
    [SerializeField] private float maxRotationAngle = 20f;
    [SerializeField] private float Offset = 90;
    private Quaternion initialPosition;

    private bool isFiring = false;
    private float fireTime = 0;

    void Start()
    {
        initialPosition = Player._PropController.getGunStickObject.transform.localRotation;
    }
    //TODO just fucking redo all of this, I asked chatgpt to help and tbh it's wasted more time than if I just did it myself lol

    // Call this method to aim the gunstick toward a target with a specific duration and cooldown time
    public void AimGunstickTowards(Vector3 target, float duration, float cooldown)
    {
        if (!IsSwivelAllowed || isFiring)
            return;

        // Calculate the direction from the gun's position to the target
        Vector3 directionToTarget = Player._PropController.getGunStickObject.transform.position - target;

        // Gradually interpolate the rotation over the specified duration
        Quaternion initialRotation = Player._PropController.getGunStickObject.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Player._PropController.getBodyBase.transform.up);
        targetRotation *= Quaternion.Euler(0, Offset, 0); // Apply the offset

        StartCoroutine(AimGunstickCoroutine(initialRotation, targetRotation, duration, cooldown));
    }

    private IEnumerator AimGunstickCoroutine(Quaternion startRotation, Quaternion targetRotation, float duration, float cooldown)
    {
        Player._PropController.StopAnimator();
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            Player._PropController.getGunStickObject.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Mark as firing and set cooldown time
        isFiring = true;
        GetComponent<AttackController>().HandleGunStickBeam();
        fireTime = Time.time;

        // Wait for the cooldown before resetting the gunstick
        yield return new WaitForSeconds(cooldown);

        // Reset gunstick to its default position
        isFiring = false;
        Player._PropController.getGunStickObject.transform.localRotation = initialPosition;
        Player._PropController.StartAnimator();
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