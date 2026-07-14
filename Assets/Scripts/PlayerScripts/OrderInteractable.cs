using UnityEngine;

public class OrderInteractable : MinigameInteractable
{
    public override void Interact()
    {
        if (!SoundManager.TutorialIsPlaying())
        {
            CustomerManager.Instance.TakeCustomerOrder();
            //move the camera into the narrative minigame if we are dealing with a named customer
            if(CustomerManager.Instance.CurrentCustomer != null)
            {
                CustomerManager.Instance.CurrentCustomer.MoveCameraOfCustomer(this);
            }
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

    public void MoveCam()
    {
        StartCoroutine(InteractSequence());
    }
}
