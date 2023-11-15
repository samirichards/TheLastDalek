using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockOnReticle : MonoBehaviour
{
    [SerializeField] private Slider _lockOnProgSlider;
    public GameObject LockOnTarget;
    public float LockOnValue = 0f;
    [SerializeField] private GameObject canvas;

    void Awake()
    {

    }

    public void SetLockOnTarget(GameObject target)
    {
        LockOnTarget = target;
        canvas.SetActive(true);
    }

    public void ClearLockOnTarget()
    {
        LockOnTarget = null;
        LockOnValue = 0f;
        canvas.SetActive(false);
    }

    void Update()
    {
        if (LockOnTarget)
        {
            var position = LockOnTarget.transform.position;
            position.y++;
            this.transform.position = position;
            _lockOnProgSlider.value = LockOnValue;
        }
    }
}
