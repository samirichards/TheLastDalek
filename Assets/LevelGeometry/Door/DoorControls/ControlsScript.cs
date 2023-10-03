using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ControlsScript : InteractiveObject
{
    public bool Hackable = false;
    public float HackTime = 3.0f;
    public float HackProgress = 0.0f;
    public GameObject DoorOpenProgressPanel;
    public GameObject HackProgressBar;
    public GameObject HackProgressBarFillColor;
    public bool InteractionBlocked = false;
    [SerializeField] private AudioClip HackingSound;
    [SerializeField] private AudioClip DenySound;

    private bool isMouseDown = false;
    private bool timingActionStarted = false; // Flag to indicate if the timing action has started
    private float timer = 0.0f;
    private bool IsHackAllowed = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseDown && !timingActionStarted && IsHackAllowed)
        {
            timingActionStarted = true; // Set flag to indicate timing action has started
        }

        if (timingActionStarted && InteractionBlocked == false)
        {
            timer += Time.deltaTime;
            HackProgress = (timer / HackTime) * 100;
            HackProgressBar.GetComponent<Slider>().value = HackProgress;

            if (HackProgress >= 100)
            {
                InteractionBlocked = true;
                timingActionStarted = false; // Reset flag when action is performed
                HackProgress = 0.0f; // Reset timer when action is performed
                timer = 0.0f;
                interactionBehavior.Interact(this.gameObject);
                HackProgressBar.GetComponent<Slider>().value = 100;
                HackProgressBarFillColor.GetComponent<Image>().color = Color.green;
            }
        }
    }

    public override void Interact(GameObject interactingCharacter)
    {
        Debug.Log(gameObject.name + " implements unique interaction behavior");
    }

    private void OnMouseDown()
    {
        
        if (Player._playerComponent.GetPrivileges())
        {
            isMouseDown = true;
            IsHackAllowed = true;
            GetComponent<AudioSource>().PlayOneShot(HackingSound);
        }
        else
        {
            IsHackAllowed = false;
            GetComponent<AudioSource>().PlayOneShot(DenySound);
            HackProgressBar.GetComponent<Slider>().value = 100;
            HackProgressBarFillColor.GetComponent<Image>().color = Color.yellow;
        }
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
        timingActionStarted = false; // Reset flag when mouse is released
        timer = 0.0f; // Reset timer when mouse is released
        HackProgress = 0.0f;
        HackProgressBarFillColor.GetComponent<Image>().color = Color.red;
        HackProgressBar.GetComponent<Slider>().value = HackProgress;
        InteractionBlocked = false;
    }

    void OnMouseEnter()
    {
        DoorOpenProgressPanel.SetActive(true);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //Hackable = player.GetComponent<InventoryManager>().EquippedItems.Count(a => a.ItemTitle == "Processor") > 0;
        player.GetComponent<AttackController>().enabled = false;
        HackProgressBar.GetComponent<Slider>().value = 0.0f;
        HackProgressBarFillColor.GetComponent<Image>().color = Color.red;
    }

    private void OnMouseExit()
    {
        DoorOpenProgressPanel.SetActive(false);
        Debug.Log("Mouse no longer over " + gameObject.name);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<AttackController>().enabled = true;
        Hackable = false;
        HackProgressBarFillColor.GetComponent<Image>().color = Color.red;
        InteractionBlocked = false;
    }
}
