using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Terminal : InteractionBehavior
{
    public bool IsInteractionAllowed { get; set; } = false;

    [SerializeField] private AudioClip InteractionSound;
    [SerializeField] private AudioClip HealSound;
    [SerializeField] private bool Charged = true;
    [SerializeField] private GameObject SparkPrefab;
    private Light _light;

    [SerializeField] public ActionType actionType = ActionType.Dumb;
    public List<DoorInteractionBehavior> ControlledDoors;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        _light = GetComponentInChildren<Light>();
    }

    public enum ActionType
    {
        ShowDialog,
        UnlockDoors,
        DisableTurrets,
        ExtendTimer,
        Recharge,
        Dumb
    }

    public override void Interact(GameObject interactingCharacter)
    {
        Debug.Log("Custom Interaction Behavior - Terminal:");
        switch (actionType)
        {
            case ActionType.ShowDialog:
                Debug.Log("Show Dialog");
                break;
            case ActionType.UnlockDoors:
                Debug.Log("Unlock Doors");
                foreach (DoorInteractionBehavior controlledDoor in ControlledDoors)
                {
                    controlledDoor.IsOpen = true;
                }
                _light.color = Color.green;
                break;
            case ActionType.DisableTurrets:
                Debug.Log("Disable Turrets");
                break;
            case ActionType.ExtendTimer:
                Debug.Log("Extend Timer");
                break;
            case ActionType.Recharge:
                Debug.Log("Replenish Health");
                if (Charged)
                {
                    interactingCharacter.GetComponent<PlayerComponent>().Health =
                        interactingCharacter.GetComponent<PlayerComponent>().MaxHealth;
                    this.GetComponent<AudioSource>().PlayOneShot(HealSound);
                    Charged = false;
                    _light.color = Color.red;
                    Vector3 sparkLocation = this.transform.position;
                    sparkLocation.y++;
                    Instantiate(SparkPrefab, sparkLocation, Quaternion.identity);
                    break;
                }
                Debug.Log("Health station empty");
                break;
            case ActionType.Dumb:
                Debug.Log("Do Nothing");
                this.GetComponent<AudioSource>().PlayOneShot(InteractionSound);
                break;
            default:
                Debug.Log("Do Nothing");
                this.GetComponent<AudioSource>().PlayOneShot(InteractionSound);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            IsInteractionAllowed = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            IsInteractionAllowed = false;
        }
    }
}
