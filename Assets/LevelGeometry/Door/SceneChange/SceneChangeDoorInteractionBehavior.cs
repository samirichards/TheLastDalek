using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeDoorInteractionBehavior : DoorInteractionBehavior
{
    [SerializeField] public BoxCollider SceneChangeTrigger;
    [SerializeField] public string TargetSceneName;
    [SerializeField] public Vector3 TargetSceneLoadLocation;
    [SerializeField] private float sceneChangeBufferDistance = 3f;
    [SerializeField] protected bool DoorEnabled = false;
    private bool targetState = false;

    void FixedUpdate()
    {
        SceneChangeTrigger.enabled = this.GetIsOpen();
        GetComponent<Animator>().SetBool("Door_IsOpen", this.GetIsOpen());
        SceneChangeTrigger.enabled = this.GetIsOpen();
        if (DoorEnabled == false)
        {
            IsOpen = false;
            if (Vector3.Distance(Player.GetPlayerReference().transform.position, this.transform.position) > sceneChangeBufferDistance)
            {
                IsOpen = targetState;
                DoorEnabled = true;
            }
        }
    }

    public override void SetIsOpen(bool _value)
    {
        targetState = _value;
        base.SetIsOpen(_value);
    }

    void Awake()
    {
        targetState = this.GetIsOpen();
    }

    void Start()
    {
        targetState = this.GetIsOpen();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && DoorEnabled)
        {
            GameManager.GetLevelTransitionManager().ChangeScene(TargetSceneLoadLocation, TargetSceneName);
        }
    }
}
