using UnityEngine;

public class ServingInteractable : MinigameInteractable
{
    public override void SetupMinigameLogic()
    {
        UI?.SetActive(true);
        UIManager.Instance.CurrentMinigameUI = UI;
        Debug.Log("Setting up Serving Minigame");
    }

    public override void Interact()
    {
        if (!SoundManager.TutorialIsPlaying())
        {
            GameManager.Instance.PushIntoMinigame();
            if(!GameManager.Instance.FirstCustomerServed)
                SoundManager.PlaySound(SoundType.Serving);
            StartCoroutine(InteractSequence());
            
        }
        
            
    }
}
