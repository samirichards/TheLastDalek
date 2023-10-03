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
        }
        else
        {
            Instantiate(SmallExplosionPrefab, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
