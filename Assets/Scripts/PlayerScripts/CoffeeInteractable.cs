using UnityEngine;

public class CoffeeInteractable : MinigameInteractable
{
    public override void SetupMinigameLogic()
    {
        base.SetupMinigameLogic();
        Debug.Log("Setting up Coffee Minigame");

    }

    public override void Interact()
    {
        if (!GameManager.Instance.TrayInHand)
        {
            GameManager.Instance.PushIntoMinigame();
            StartCoroutine(InteractSequence());
        }
    }
}
