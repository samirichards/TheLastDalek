using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRotateController : MonoBehaviour
{
    [SerializeField] public bool IsRotationAllowed = false;
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
                Vector3 targetPosTurret = new Vector3(targetPos.x, Player._PropController.getCenterObject.transform.position.y, targetPos.z);
                Quaternion turretRotationFinal = Quaternion.LookRotation(
                forward: targetPosTurret - Player._PropController.getCenterObject.transform.position,
                upwards: Player._PropController.getBodyBase.transform.up);

                float turretDegreesToFinal = Quaternion.Angle(Player._PropController.getCenterObject.transform.rotation, turretRotationFinal);
                Player._PropController.getCenterObject.transform.rotation = Quaternion.Lerp(
                    Player._PropController.getCenterObject.transform.rotation, turretRotationFinal,
                (rotationSpeed / turretDegreesToFinal) * Time.deltaTime);
            }
            else
            {
                Player._PropController.getCenterObject.transform.rotation = Player._PropController.getBodyBase.transform.rotation;
                if (!LookAnimatorDisabled)
                {
                    lookatAnimator.ClearLookTarget();
                    LookAnimatorDisabled = true;
                }
            }
        }

    }
}
