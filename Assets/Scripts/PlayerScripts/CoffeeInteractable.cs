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
        if (!GameManager.Instance.DrinksInHand)
        {
            GameManager.Instance.PushIntoMinigame();
            StartCoroutine(InteractSequence());
        }

        if (GameManager.Instance.TutorialNoises != null)
            SoundManager.PlaySound(SoundType.Drinks);
    }
}
