using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStickAimController : MonoBehaviour
{
    [SerializeField] public bool IsSwivelAllowed = false;
    [SerializeField] private GameObject GunStick;
    [SerializeField] private GameObject BodyBase;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float maxRotationAngle = 20f;
    private GameManager _gameManagerComponent;
    // Start is called before the first frame update
    void Start()
    {
        _gameManagerComponent = GetComponent<GameManager>();
    }

    private void Update()
    {
        if (!_gameManagerComponent.IsGamePaused)
        {
            if (IsSwivelAllowed)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 200);
                Vector3 targetPos = hit.point;

                Vector3 gunstickDirection = targetPos - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(gunstickDirection, transform.up);
                Quaternion limitedRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                limitedRotation = LimitRotation(limitedRotation, maxRotationAngle);
                GunStick.transform.rotation = limitedRotation;
            }
            else
            {

            }
        }
    }

    private Quaternion LimitRotation(Quaternion rotation, float maxAngle)
    {
        Quaternion limitedRotation = rotation;
        float angle = Quaternion.Angle(transform.rotation, rotation);
        if (angle > maxAngle)
        {
            Quaternion maxRotation = Quaternion.RotateTowards(transform.rotation, rotation, maxAngle);
            limitedRotation = Quaternion.RotateTowards(maxRotation, rotation, -maxAngle);
        }
        return limitedRotation;
    }
}
