using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private float Damage = 70f;
    [SerializeField] private GameObject ExplosionPrefab;
    [SerializeField] private MeshRenderer renderer;

    [SerializeField] private bool IsRevealed = false;
    // Start is called before the first frame update

    public void SetVisibility(bool visible)
    {
        IsRevealed = visible;
        renderer.enabled = IsRevealed;
    }

    private void OnValidate()
    {
        renderer.enabled = IsRevealed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerComponent>())
        {
            other.GetComponent<PlayerComponent>().Damage(new DamageInfo(Damage, gameObject, DamageType.Point));
            Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
