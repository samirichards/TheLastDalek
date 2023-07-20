using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public InteractionBehavior interactionBehavior;

    public virtual void Interact(GameObject interactingCharacter)
    {
        if (interactionBehavior != null)
        {
            interactionBehavior.Interact(interactingCharacter);
        }
    }
}
