using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShieldManager : MonoBehaviour
{

    public Animator ShieldAnimator;
    public float ShieldMaxHealth = 100f;
    public float ShieldHealth;
    public float ShieldRechargeRate = 6.66f;
    public float ShieldRechargeDelay = 3f;
    private float RechargeDelayTimer = 0.0f;
    private bool IsRecharging = false;
    public bool ShieldEnabled = false;
    public bool ShieldEffective = true;
    // Start is called before the first frame update
    void Start()
    {
        ShieldHealth = ShieldMaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        ShieldEffective = ShieldHealth > 0 && ShieldEnabled;
        if (ShieldHealth < ShieldMaxHealth && RechargeDelayTimer <= 0)
        {
            ShieldHealth += ShieldRechargeRate * Time.deltaTime;
        }
        else
        {
            if (RechargeDelayTimer > 0)
            {
                RechargeDelayTimer -= Time.deltaTime;
            }
        }

        ShieldAnimator.SetBool("IsActive", ShieldHealth > 0 && ShieldEnabled);
    }

    public void ShieldSetActive()
    {
        ShieldAnimator.Play(Animator.StringToHash("OpenShield"));
        ShieldAnimator.SetBool("IsActive", true);
    }

    public void ShieldSetInactive()
    {
        ShieldAnimator.Play(Animator.StringToHash("CloseShield"));
    }

    public void DamageShield(DamageInfo info)
    {
        RechargeDelayTimer = ShieldRechargeDelay;
        ShieldHealth -= info.DamageValue;
        if (ShieldHealth < 0)
        {
            ShieldHealth = 0;
            ShieldSetInactive();
        }
        ShieldAnimator.Play(Animator.StringToHash("ShieldDamage"));
    }
}
