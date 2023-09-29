using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public GameObject HoleBase;
    public BoxCollider Collider;


    void Awake()
    {
        Collider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Movement>().IsElevating)
        {
            Physics.IgnoreCollision(collision.collider, Collider);
        }
    }
}
