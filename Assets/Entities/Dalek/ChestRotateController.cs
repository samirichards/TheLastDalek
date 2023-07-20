using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRotateController : MonoBehaviour
{
    [SerializeField] public bool IsRotationAllowed = false;
    [SerializeField] private GameObject ChestSection;
    [SerializeField] private GameObject BodyBase;
    [SerializeField] private float rotationSpeed = 1f;
    private LookAtAnimator lookatAnimator;
    private bool LookAnimatorDisabled = false;
    // Start is called before the first frame update
    void Start()
    {
        lookatAnimator = GetComponent<LookAtAnimator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameManager.IsGamePaused)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 200);
            if (IsRotationAllowed)
            {
                LookAnimatorDisabled = false;
                Vector3 targetPos = hit.point;
                lookatAnimator.SetLookTarget(targetPos);

                // turret control:
                Vector3 targetPosTurret = new Vector3(targetPos.x, ChestSection.transform.position.y, targetPos.z);
                Quaternion turretRotationFinal = Quaternion.LookRotation(
                forward: targetPosTurret - ChestSection.transform.position,
                upwards: BodyBase.transform.up);

                float turretDegreesToFinal = Quaternion.Angle(ChestSection.transform.rotation, turretRotationFinal);
                ChestSection.transform.rotation = Quaternion.Lerp(
                ChestSection.transform.rotation, turretRotationFinal,
                (rotationSpeed / turretDegreesToFinal) * Time.deltaTime);
            }
            else
            {
                ChestSection.transform.rotation = BodyBase.transform.rotation;
                if (!LookAnimatorDisabled)
                {
                    lookatAnimator.ClearLookTarget();
                    LookAnimatorDisabled = true;
                }
            }
        }

    }
}
