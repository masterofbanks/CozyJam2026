using UnityEngine;

public class GiveOrderInteractable : Interactable
{
    public CustomerBehavior CustomerBehaviorScript;

    private void Awake()
    {

        CustomerBehaviorScript = GetComponentInParent<CustomerBehavior>();
    }
    public override void Interact()
    {
        if (!CustomerBehaviorScript.agent.updatePosition)
        {
            CustomerBehaviorScript.GiveFood(GameManager.Instance.OrderInHand);
        }
    }
}
