using UnityEngine;

public class OrderInteractable : Interactable
{
    public override void Interact()
    {
        CustomerManager.Instance.TakeCustomerOrder();
        if (GameManager.Instance.TutorialNoises != null)
        {
            if (!GameManager.Instance.FirstCustomerServed)
                SoundManager.PlaySound(SoundType.TakeOrder);
        }
            
    }
}
