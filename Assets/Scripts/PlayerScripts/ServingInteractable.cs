using UnityEngine;

public class ServingInteractable : MinigameInteractable
{
    public override void SetupMinigameLogic()
    {
        UI?.SetActive(true);
        UIManager.Instance.CurrentMinigameUI = UI;
        Debug.Log("Setting up Mixing Minigame");
    }

    public override void Interact()
    {
        GameManager.Instance.PushIntoMinigame();
        StartCoroutine(InteractSequence());
    }
}
