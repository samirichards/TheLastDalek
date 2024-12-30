using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepController : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioClip[] StepSoundsConcrete;
    [SerializeField] protected AudioClip[] StepSoundsTile;
    [SerializeField] protected AudioClip[] StepSoundsMetal;
    [SerializeField] protected AudioClip[] StepSoundsGrate;
    [SerializeField] protected AudioClip[] StepSoundsHollow;
    [SerializeField] protected AudioClip[] StepSoundsDirt;
    [SerializeField] protected AudioClip[] StepSoundsWet;
    [SerializeField] protected AudioClip[] StepSoundsWater;
    [SerializeField] private float walkVolume = 0.4f;

    private AudioSource audioSource;
    public void Awake()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }
    public void FootStep(AnimationEvent evt)
    {
        //- is left foot, + is right foot, 1 is walk, 2 is run
        float stepVolume = evt.intParameter < -1 || evt.intParameter > 1 ? 1f : walkVolume;
        if (evt.animatorClipInfo.weight < 0.666f)
        {
            return;
        }
        Debug.DrawRay((transform.position + Vector3.up), Vector3.down * 2f, Color.red, 2f);
        RaycastHit hit;
        if (Physics.Raycast((transform.position + Vector3.up), Vector3.down, out hit, 2f, LayerMask.GetMask("Ground", "Water", "Floor")))
        {
            var surfaceInfo = hit.collider.GetComponent<WalkableSurface>();
            if (surfaceInfo != null)
            {
                switch (surfaceInfo.GetMaterialType())
                {
                    case Materials.MaterialType.None:
                        break;
                    case Materials.MaterialType.Concrete:
                        audioSource.PlayOneShot(StepSoundsConcrete[Mathf.RoundToInt(Random.Range(0, StepSoundsConcrete.Length))], stepVolume);
                        break;
                    case Materials.MaterialType.Tile:
                        audioSource.PlayOneShot(StepSoundsTile[Mathf.RoundToInt(Random.Range(0, StepSoundsTile.Length))], stepVolume);
                        break;
                    case Materials.MaterialType.Metal:
                        audioSource.PlayOneShot(StepSoundsMetal[Mathf.RoundToInt(Random.Range(0, StepSoundsMetal.Length))], stepVolume);
                        break;
                    case Materials.MaterialType.Grate:
                        audioSource.PlayOneShot(StepSoundsGrate[Mathf.RoundToInt(Random.Range(0, StepSoundsGrate.Length))], stepVolume);
                        break;
                    case Materials.MaterialType.Hollow:
                        audioSource.PlayOneShot(StepSoundsHollow[Mathf.RoundToInt(Random.Range(0, StepSoundsHollow.Length))], stepVolume);
                        break;
                    case Materials.MaterialType.Dirt:
                        audioSource.PlayOneShot(StepSoundsDirt[Mathf.RoundToInt(Random.Range(0, StepSoundsDirt.Length))], stepVolume);
                        break;
                    case Materials.MaterialType.Wet:
                        audioSource.PlayOneShot(StepSoundsWet[Mathf.RoundToInt(Random.Range(0, StepSoundsWet.Length))], stepVolume);
                        break;
                    case Materials.MaterialType.Water:
                        audioSource.PlayOneShot(StepSoundsWater[Mathf.RoundToInt(Random.Range(0, StepSoundsWater.Length))], stepVolume);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
