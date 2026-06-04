using UnityEngine;

public class OrderInteractable : Interactable
{
    public override void Interact()
    {
        if (!SoundManager.TutorialIsPlaying())
        {
            CustomerManager.Instance.TakeCustomerOrder();
            if (GameManager.Instance.TutorialNoises != null && !TutorialPlayed)
            {
                if (!GameManager.Instance.FirstCustomerServed)
                {
                    SoundManager.PlaySound(SoundType.TakeOrder);
                    TutorialPlayed = true;
                }
            }
        }
        
            
    }
}
