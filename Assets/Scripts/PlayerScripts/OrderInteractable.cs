using UnityEngine;

public class OrderInteractable : Interactable
{
    public override void Interact()
    {
        CustomerManager.Instance.TakeCustomerOrder();
        if (GameManager.Instance.TutorialNoises != null)
            SoundManager.PlaySound(SoundType.TakeOrder, 1.0f);
    }
}
