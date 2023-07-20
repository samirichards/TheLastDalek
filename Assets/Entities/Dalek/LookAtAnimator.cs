using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtAnimator : MonoBehaviour
{
    [SerializeField] Vector3? LookatTarget;
    [SerializeField] GameObject HeadItem;
    [SerializeField] GameObject EyeStalk;
    [SerializeField] GameObject BodyBase;
    [SerializeField] float rotationSpeed = 180f;
    [SerializeField] float EyestalkMinAngle = -30;
    [SerializeField] float EyestalkMaxAngle = 150;
    [SerializeField] float EyestalkMoveSpeed = 60f;

    private Quaternion _lookRotation;
    private Vector3 _direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LookatTarget != null)
        {
            Vector3 targetPos = (Vector3)LookatTarget;

            // turret control:
            Vector3 targetPosTurret = new Vector3(targetPos.x, HeadItem.transform.position.y, targetPos.z);
            Quaternion turretRotationFinal = Quaternion.LookRotation(
                forward: targetPosTurret - HeadItem.transform.position,
                upwards: BodyBase.transform.up
            );
            float turretDegreesToFinal = Quaternion.Angle(HeadItem.transform.rotation, turretRotationFinal);
            HeadItem.transform.rotation = Quaternion.Lerp(
                HeadItem.transform.rotation, turretRotationFinal,
                (rotationSpeed / turretDegreesToFinal) * Time.deltaTime
            );

            Vector3 targetPosEye = HeadItem.transform.InverseTransformPoint(targetPos);
            float gunAngleFinal = -Mathf.Atan2(targetPosEye.y, targetPosEye.z) * Mathf.Rad2Deg;
            float gunAngleLimited = Mathf.Clamp(gunAngleFinal, EyestalkMinAngle, EyestalkMaxAngle);
            EyeStalk.transform.localRotation = Quaternion.Lerp(
                EyeStalk.transform.localRotation,
                Quaternion.Euler(gunAngleLimited, 0, 0),
                Mathf.Abs(EyestalkMoveSpeed / gunAngleLimited) * Time.deltaTime
            );
        }
        else
        {
            HeadItem.transform.rotation = BodyBase.transform.rotation;
        }
    

    }

    public void SetLookTarget(GameObject gameObject)
    {
        LookatTarget = gameObject.transform.position;
    }

    public void SetLookTarget(Transform transform)
    {
        LookatTarget = transform.position;
    }

    public void SetLookTarget(Vector3 position)
    {
        LookatTarget = position;
    }

    public void ClearLookTarget()
    {
        LookatTarget = null;
    }
}
