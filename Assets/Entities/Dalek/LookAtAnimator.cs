using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtAnimator : MonoBehaviour
{
    [SerializeField] Vector3? LookatTarget;
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
            Vector3 targetPosTurret = new Vector3(targetPos.x, Player._PropController.getHeadObject.transform.position.y, targetPos.z);
            Quaternion turretRotationFinal = Quaternion.LookRotation(
                forward: targetPosTurret - Player._PropController.getHeadObject.transform.position,
                upwards: Player._PropController.getBodyBase.transform.up
            );
            float turretDegreesToFinal = Quaternion.Angle(Player._PropController.getHeadObject.transform.rotation, turretRotationFinal);
            Player._PropController.getHeadObject.transform.rotation = Quaternion.Lerp(
                Player._PropController.getHeadObject.transform.rotation, turretRotationFinal,
                (rotationSpeed / turretDegreesToFinal) * Time.deltaTime
            );

            Vector3 targetPosEye = Player._PropController.getHeadObject.transform.InverseTransformPoint(targetPos);
            float gunAngleFinal = -Mathf.Atan2(targetPosEye.y, targetPosEye.z) * Mathf.Rad2Deg;
            float gunAngleLimited = Mathf.Clamp(gunAngleFinal, EyestalkMinAngle, EyestalkMaxAngle);
            Player._PropController.getEyeStalkObject.transform.localRotation = Quaternion.Lerp(
                Player._PropController.getEyeStalkObject.transform.localRotation,
                Quaternion.Euler(gunAngleLimited, 0, 0),
                Mathf.Abs(EyestalkMoveSpeed / gunAngleLimited) * Time.deltaTime
            );
        }
        else
        {
            Player._PropController.getHeadObject.transform.rotation = Player._PropController.getBodyBase.transform.rotation;
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
