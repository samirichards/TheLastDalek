using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public float interactionDistance = 2f;

    public GameObject InteractionPoint;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Debug.DrawRay(InteractionPoint.transform.position, InteractionPoint.transform.forward * interactionDistance, Color.grey, 5);
            if (Physics.Raycast(InteractionPoint.transform.position, InteractionPoint.transform.forward, out hit, interactionDistance))
            {
                InteractiveObject interactiveObject = hit.collider.GetComponent<InteractiveObject>();
                if (interactiveObject != null)
                {
                    interactiveObject.Interact(gameObject);
                }
                else
                {
                    Debug.Log("Interaction Raycast hit non interactable object, hit " + hit.collider.gameObject.name);
                }
            }
            else
            {
                Debug.Log("Interaction Raycast hit nothing");
            }
        }
    }
}
