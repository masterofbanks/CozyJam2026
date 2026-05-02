using UnityEngine;

public class OrderInteractable : Interactable
{
    public override void Interact()
    {
        CustomerManager.Instance.TakeCustomerOrder();
    }
}
