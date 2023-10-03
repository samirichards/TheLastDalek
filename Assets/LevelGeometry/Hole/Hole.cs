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

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Movement>().CanElevate)
        {
            other.gameObject.GetComponent<Movement>().ElevationTargets.Add(gameObject);
            Collider.enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Movement>())
        {
            if (other.gameObject.GetComponent<Movement>().ElevationTargets.Contains(gameObject))
            {
                other.gameObject.GetComponent<Movement>().ElevationTargets.Remove(gameObject);
            }
            Collider.enabled = true;
        }
    }
}
