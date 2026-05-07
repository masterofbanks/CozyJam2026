using UnityEngine;

public class MixingMinigameInteractable : MinigameInteractable
{
    public override void SetupMinigameLogic()
    {
        base.SetupMinigameLogic();
        Debug.Log("Setting up Mixing Minigame");
    }

    public override void Interact()
    {
        if (!GameManager.Instance.TrayInHand && !GameManager.Instance.TrayIsCooked && !GameManager.Instance.TrayInOven)
        {
            GameManager.Instance.PushIntoMinigame();
            StartCoroutine(InteractSequence());
            if (GameManager.Instance.TutorialNoises != null)
            {
                if (!GameManager.Instance.FirstCustomerServed)
                    SoundManager.PlaySound(SoundType.Mixing);
            }
                
        }
    }
}

