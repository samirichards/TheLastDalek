using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ExplosiveBarrel : DamageableComponent
{
    [SerializeField]public bool LeaveHole = false;
    [SerializeField] public GameObject HoleModel;
    [SerializeField] public GameObject SmallExplosionPrefab;
    [SerializeField] public GameObject LargeExplosionPrefab;
    [SerializeField] private float ExplosionRange = 7f;
    [SerializeField] private float ExplosionDamage = 8f;
    [SerializeField] private float LargeExplosionDamageMultipler = 1.2f;
    [SerializeField] private float LargeExplosionRangeMultiplier = 1.15f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnBreak(DamageInfo damageInfo)
    {
        if (LeaveHole)
        {
            Instantiate(HoleModel, transform.position, transform.rotation);
            Instantiate(LargeExplosionPrefab, transform.position, transform.rotation);
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, ExplosionRange * LargeExplosionRangeMultiplier);
            foreach (Collider hit in colliders)
            {
                if (hit.gameObject != gameObject && hit.transform.root.gameObject != gameObject)
                {
                    Rigidbody rb = hit.GetComponent<Rigidbody>();

                    if (rb != null)
                        rb.AddExplosionForce(15f, explosionPos, ExplosionRange * LargeExplosionRangeMultiplier, 0f, ForceMode.Impulse);

                    //Letting the explosions trigger other explosions would look cool, however, it also crashes the game, so for now the fun can't be allowed
                    if (hit.GetComponent<DamageableComponent>() != null && hit.GetComponent<ExplosiveBarrel>() == null)
                    {
                        hit.GetComponent<DamageableComponent>().Damage(new DamageInfo(
                            Mathf.Lerp(ExplosionDamage * LargeExplosionDamageMultipler, 0f, Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange * LargeExplosionRangeMultiplier)), this.gameObject, DamageType.Explosion, hit.transform));
                        Debug.Log($"{this.gameObject} exploding did {Mathf.Round(Mathf.Lerp(ExplosionDamage * LargeExplosionDamageMultipler, 0f, Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange * LargeExplosionRangeMultiplier)))} damage to {hit.gameObject} (Mathf.Lerp({ExplosionDamage * LargeExplosionDamageMultipler}, {0f}, {Mathf.Round(Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange * LargeExplosionRangeMultiplier))}))");
                    }
                    else if (hit.GetComponent<BaseAI>() != null)
                    {
                        hit.GetComponent<BaseAI>().Damage(new DamageInfo(
                            Mathf.Lerp(ExplosionDamage * LargeExplosionDamageMultipler, 0f, Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange * LargeExplosionRangeMultiplier)), this.gameObject, DamageType.Explosion, hit.transform));
                        Debug.Log($"{this.gameObject} exploding did {Mathf.Round(Mathf.Lerp(ExplosionDamage * LargeExplosionDamageMultipler, 0f, Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange * LargeExplosionRangeMultiplier)))} damage to {hit.gameObject} (Mathf.Lerp({ExplosionDamage * LargeExplosionDamageMultipler}, {0f}, {Mathf.Round(Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange * LargeExplosionRangeMultiplier))}))");
                    }
                }
            }
        }
        else
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, ExplosionRange);
            foreach (Collider hit in colliders)
            {
                if (hit.gameObject != gameObject && hit.transform.root.gameObject != gameObject)
                {
                    Rigidbody rb = hit.GetComponent<Rigidbody>();

                    if (rb != null)
                        rb.AddExplosionForce(8f, explosionPos, ExplosionRange, 0f, ForceMode.Impulse);

                    if (hit.GetComponent<DamageableComponent>() != null && hit.GetComponent<ExplosiveBarrel>() == null)
                    {
                        hit.GetComponent<DamageableComponent>().Damage(new DamageInfo(
                            Mathf.Lerp(ExplosionDamage, 0f, Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange)), this.gameObject, DamageType.Explosion, hit.transform));
                        Debug.Log($"{this.gameObject} exploding did {Mathf.Round(Mathf.Lerp(ExplosionDamage * LargeExplosionDamageMultipler, 0f, Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange * LargeExplosionRangeMultiplier)))} damage to {hit.gameObject} (Mathf.Lerp({ExplosionDamage * LargeExplosionDamageMultipler}, {0f}, {Mathf.Round(Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange * LargeExplosionRangeMultiplier))}))");
                    }
                    else if (hit.GetComponent<BaseAI>() != null)
                    {
                        hit.GetComponent<BaseAI>().Damage(new DamageInfo(
                            Mathf.Lerp(ExplosionDamage, 0f, Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange)), this.gameObject, DamageType.Explosion, hit.transform));
                        Debug.Log($"{this.gameObject} exploding did {Mathf.Round(Mathf.Lerp(ExplosionDamage * LargeExplosionDamageMultipler, 0f, Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange * LargeExplosionRangeMultiplier)))} damage to {hit.gameObject} (Mathf.Lerp({ExplosionDamage * LargeExplosionDamageMultipler}, {0f}, {Mathf.Round(Vector3.Distance(this.transform.position, hit.transform.position) / (ExplosionRange * LargeExplosionRangeMultiplier))}))");
                    }
                }
            }
            Instantiate(SmallExplosionPrefab, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
