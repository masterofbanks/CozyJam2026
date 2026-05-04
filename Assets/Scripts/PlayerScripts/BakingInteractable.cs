using UnityEngine;

public class BakingInteractable : Interactable
{
    [SerializeField] private float TimeNeededToCookTray = 30f;
    [SerializeField] private GameObject AlarmSFX;
    private float t = 0f;
    private bool TrayInOven = false;
    private bool TrayFinishedCooking = false;

    public override void Interact()
    {
        if (GameManager.Instance.TrayInHand && !GameManager.Instance.TrayIsCooked)
        {
            GameManager.Instance.ThrowTrayInOven();
            TrayInOven = true;
        }

        else if (TrayFinishedCooking)
        {
            TrayFinishedCooking = false;
            TrayInOven = false;
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
        if (TrayInOven)
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
        TrayInOven = false;
        t = 0;
        Debug.Log("Tray Finished Cooking");
        TrayFinishedCooking = true;
        Instantiate(AlarmSFX, transform.position, Quaternion.identity);
    }
}

