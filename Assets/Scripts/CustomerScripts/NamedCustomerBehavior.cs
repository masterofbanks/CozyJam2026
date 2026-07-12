using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class NamedCustomerBehavior : CustomerBehavior
{
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anime = GetComponent<Animator>();
        anime.runtimeAnimatorController = anims.GetRandomCharacterController();
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
    }


    public override void GiveOrder(CustomerPreset preset, int id)
    {
        base.GiveOrder(preset, id);
        CustomerName = preset.CustomerName;
        NameTextBox.text = CustomerName;
        anime = GetComponent<Animator>();
        anime.runtimeAnimatorController = anims.GetCharacterController(preset.AppearanceIndex);
    }

    public override void TakeOrder()
    {
        Debug.Log("You have tried to take a named customer's order");
    }
    //
}
