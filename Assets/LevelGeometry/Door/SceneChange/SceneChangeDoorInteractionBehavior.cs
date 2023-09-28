using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeDoorInteractionBehavior : DoorInteractionBehavior
{
    [SerializeField] public BoxCollider SceneChangeTrigger;
    [SerializeField] public string TargetSceneName;
    [SerializeField] public Vector3 TargetSceneLoadLocation;
    public override void Interact(GameObject interactingCharacter)
    {

    }

    void FixedUpdate()
    {
        SceneChangeTrigger.enabled = IsOpen;
        GetComponent<Animator>().SetBool("Door_IsOpen", IsOpen);
        SceneChangeTrigger.enabled = IsOpen;
    }

    void Awake()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            SceneManager.LoadScene(TargetSceneName);
            other.transform.position = TargetSceneLoadLocation;
        }
    }
}
