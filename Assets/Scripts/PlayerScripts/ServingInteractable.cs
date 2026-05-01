using UnityEngine;

public class ServingInteractable : MinigameInteractable
{
    public override void SetupMinigameLogic()
    {
        base.SetupMinigameLogic();
        Debug.Log("Setting up Mixing Minigame");
    }

    public override void Interact()
    {
        if (GameManager.Instance.TrayInHand && GameManager.Instance.TrayIsCooked)
        {
            GameManager.Instance.PushIntoMinigame();
            StartCoroutine(InteractSequence());
        }
    }
}
