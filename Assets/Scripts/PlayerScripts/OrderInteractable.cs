using UnityEngine;

public class OrderInteractable : Interactable
{
    public override void Interact()
    {
        if (!SoundManager.TutorialIsPlaying() && !TutorialPlayed)
        {
            CustomerManager.Instance.TakeCustomerOrder();
            if (GameManager.Instance.TutorialNoises != null)
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
