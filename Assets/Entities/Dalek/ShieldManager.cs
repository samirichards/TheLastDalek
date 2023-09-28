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
    public int ShieldTier = 0;
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

    /// <summary>
    /// Enables the shield
    /// 200 max charges with recharge rate of 13.333 per second with tier 1
    /// 100 max charges with recharge rate of 6.666 per seconds with tier 0
    /// </summary>
    public void ShieldSetActive()
    {
        ShieldAnimator.Play(Animator.StringToHash("OpenShield"));
        ShieldAnimator.SetBool("IsActive", true);
        if (ShieldTier > 0)
        {
            //Enhanced Shield behavior
            ShieldMaxHealth = 200;
            ShieldRechargeDelay = 2f;
            ShieldRechargeRate = 13.333f;
            //From 0 to 200, this gives an effective recharge time of 15 seconds
            ShieldEnabled = true;
            ShieldEffective = true;
        }
        else
        {
            //Default shield behavior
            ShieldMaxHealth = 100;
            ShieldRechargeDelay = 3f;
            ShieldRechargeRate = 6.666f;
            //From 0 to 100, this gives an effective recharge time of 15 seconds
            ShieldEnabled = true;
            ShieldEffective = true;
        }
    }

    public void ShieldSetInactive()
    {
        ShieldAnimator.Play(Animator.StringToHash("CloseShield"));
        ShieldEnabled = false;
        ShieldEffective = false;
    }

    public void SetShieldDisabled()
    {
        ShieldAnimator.Play(Animator.StringToHash("CloseShield"));
        ShieldEffective = false;
    }

    public void DamageShield(DamageInfo info)
    {
        RechargeDelayTimer = ShieldRechargeDelay;
        ShieldHealth -= info.DamageValue;
        if (ShieldHealth < 0)
        {
            ShieldHealth = 0;
            SetShieldDisabled();
        }
        ShieldAnimator.Play(Animator.StringToHash("ShieldDamage"));
    }
}
