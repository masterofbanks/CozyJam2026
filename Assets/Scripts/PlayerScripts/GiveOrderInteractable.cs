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
        if (!CustomerBehaviorScript.agent.updatePosition && GameManager.Instance.OrdersInHand.Count > 0)
        {
            CustomerBehaviorScript.GiveFood(GameManager.Instance.OrdersInHand[0]);

        }
    }
}
