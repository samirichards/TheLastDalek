using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpecialWeaponsDalekPropController: PropController
{
    [Header("Special Weapons Dalek Specific Properties")]
    [SerializeField] private Material inactiveEmittersMaterial;
    [SerializeField] private Material activeEmittersMaterial;
    [SerializeField] private MeshRenderer emitters;
    [SerializeField] private float ShootCameraShakeDuration;
    [SerializeField] private float ShootCameraShakeIntensity;

    public override void SetEmittersActive(bool state)
    {
        if (state)
        {
            //emitters.materials = emitters.materials.Where(a => a.name != inactiveEmittersMaterial.name).Append(activeEmittersMaterial).ToArray();
            emitters.materials[1] = activeEmittersMaterial;
        }
        else
        {
            //emitters.materials = emitters.materials.Where(a => a.name != activeEmittersMaterial.name).Append(inactiveEmittersMaterial).ToArray();
            emitters.materials[1] = inactiveEmittersMaterial;
        }
    }

    public override void OnFire()
    {
        GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>().StartCameraShake(ShootCameraShakeDuration, ShootCameraShakeIntensity);
        base.OnFire();
    }
}
