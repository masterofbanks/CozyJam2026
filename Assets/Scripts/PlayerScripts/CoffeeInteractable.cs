using UnityEngine;

public class CoffeeInteractable : MinigameInteractable
{
    public override void SetupMinigameLogic()
    {
        base.SetupMinigameLogic();
        Debug.Log("Setting up Coffee Minigame");

    }

   
    private void Start()
    {
        GameManager.Instance.BrewedSomeDrinks += PutXOnMinigame;
        GameManager.Instance.ServedOrderDrinksAction += RemoveXFromMinigame;
    }

    public override void Interact()
    {
        if (!SoundManager.TutorialIsPlaying())
        {
            if (!GameManager.Instance.DrinksInHand)
            {
                GameManager.Instance.PushIntoMinigame();
                StartCoroutine(InteractSequence());
            }

            if (GameManager.Instance.TutorialNoises != null)
            {
                if (!GameManager.Instance.FirstCustomerServed)
                    SoundManager.PlaySound(SoundType.Drinks);

            }
        }
        
    }

    

}
