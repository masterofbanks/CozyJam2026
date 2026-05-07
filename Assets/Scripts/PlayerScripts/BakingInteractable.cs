using UnityEngine;

public class BakingInteractable : Interactable
{
    [SerializeField] private float TimeNeededToCookTray = 30f;
    [SerializeField] private GameObject CookingSFX;
    private float t = 0f;
    private bool TrayFinishedCooking = false;

    public override void Interact()
    {
        if (GameManager.Instance.TrayInHand && !GameManager.Instance.TrayIsCooked)
        {
            GameManager.Instance.ThrowTrayInOven();
            Instantiate(CookingSFX, transform.position, Quaternion.identity);
        }

        else if (TrayFinishedCooking)
        {
            TrayFinishedCooking = false;
            GameManager.Instance.PutTrayInHand();
            GameManager.Instance.TrayIsCooked = true;
        }

        if(GameManager.Instance.TutorialNoises != null)
        {
            if (!GameManager.Instance.FirstCustomerServed)
                SoundManager.PlaySound(SoundType.Oven);

        }
    }

    private void Update()
    {
        if (GameManager.Instance.TrayInOven)
        {
            t += Time.deltaTime;
            if(t > TimeNeededToCookTray)
            {
                FinishedCooking();
            }
        }
    }

    private void FinishedCooking()
    {
        t = 0;
        Debug.Log("Tray Finished Cooking");
        TrayFinishedCooking = true;
    }
}

