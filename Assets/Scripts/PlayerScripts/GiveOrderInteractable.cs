using System;
using UnityEngine;

public class GiveOrderInteractable : Interactable
{
    public CustomerBehavior CustomerBehaviorScript;

    private void Awake()
    {

        CustomerBehaviorScript = GetComponentInParent<CustomerBehavior>();
    }

    /// <summary>
    /// Give the first order in the player's hand to a customer
    /// </summary>
    public override void Interact()
    {
        //player needs to be standing still and have orders in hand to give an order to a customer
        if (!CustomerBehaviorScript.agent.updatePosition && GameManager.Instance.OrdersInHand.Count > 0)
        {
            Tuple<string, string> order = GameManager.Instance.OrdersInHand[0];
            string finalOrder = $"{order.Item1}-{order.Item2}"; //format is drinks order, food order

            CustomerBehaviorScript.GiveFood(finalOrder);

        }
    }
}
