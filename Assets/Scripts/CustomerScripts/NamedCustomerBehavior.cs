using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class NamedCustomerBehavior : CustomerBehavior
{
    [Header("Named Stuff")]
    [SerializeField] private List<DialogueSequence> DaysOfDialogue;
    [SerializeField] private int CurrentDayOfDialogue;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anime = GetComponent<Animator>();
       /* int temp = 0;
        anime.runtimeAnimatorController = anims.GetRandomCharacterController(out temp);
        AnimationID = temp;*/
        animCatScript = new AnimateCat(anime);
        stateMachine = new CustomerStateMachine(this);
        IDText = GetComponentInChildren<TextMeshProUGUI>();
        GameManager.Instance.FreezeCustomers += FreezeCustomer;
        GameManager.Instance.UnfreezeCustomers += UnfreezeCustomer;
    }

    private void Start()
    {
        stateMachine.Initialize(stateMachine.StartState);
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        CurrentRating = MaxHappinessRating;

        //TODO: FIND THE CORRECT DAYS OF DIALGOUE AND CURRENT DAY OF DIALOGUE FOR THIS NAMED CHARACTER
    }


    public override void GiveOrder(CustomerPreset preset, int id)
    {
        base.GiveOrder(preset, id);
        CustomerName = preset.CustomerName;
        NameTextBox.text = CustomerName;
        anime = GetComponent<Animator>();
        anime.runtimeAnimatorController = anims.GetCharacterController(preset.AppearanceIndex);
        animCatScript = new AnimateCat(anime);
        stateMachine = new CustomerStateMachine(this);
        AnimationID = preset.AppearanceIndex;
    }

    public override void TakeOrder()
    {
        Debug.Log("You have tried to take a named customer's order");
    }

    public override void MoveCameraOfCustomer(OrderInteractable interactable)
    {
        interactable.MoveCam();
    }


    /// <summary>
    /// Find the current dialogue sequence to play within the narrative minigame.
    /// </summary>
    /// <returns>The current day's dialgoue sequence for this named customer</returns>
    public override DialogueSequence GetDialogueSequence()
    {
        return DaysOfDialogue[CurrentDayOfDialogue];
    }


    //TODO: Named customer's rating does not decrease while talking to the player
}
