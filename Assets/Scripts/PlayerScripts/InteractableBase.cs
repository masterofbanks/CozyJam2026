using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact();
}

public abstract class MinigameInteractable : Interactable
{
    [Header("Camera Information")]
    [SerializeField] private float CameraDelay = 1.0f;
    [SerializeField] private GameObject GameCamera;
    [SerializeField] private GameObject MinigameCamera;

    [Header("UI")]
    [SerializeField] protected GameObject UI;
    private void OpenCamera()
    {
        GameCamera.SetActive(false);
        MinigameCamera.SetActive(true);
        UIManager.Instance.RemoveBlackFromScreen();
    }

    private void CloseCamera()
    {
        UIManager.Instance.AddBlackToScreen();
    }

    public virtual void SetupMinigameLogic()
    {
        UIManager.Instance.ToggleDefaultMinigameUI();
        UI?.SetActive(true);
        UIManager.Instance.CurrentMinigameUI = UI;
    }
    public override void Interact()
    {
        GameManager.Instance.PushIntoMinigame();
        StartCoroutine(InteractSequence());
    }

    private IEnumerator InteractSequence()
    {
        CloseCamera();
        yield return new WaitForSeconds(CameraDelay);
        SetupMinigameLogic();
        yield return new WaitForSeconds(CameraDelay);
        OpenCamera();
        yield return new WaitForSeconds(CameraDelay);
        UIManager.Instance.RemoveFader();
    }
}

