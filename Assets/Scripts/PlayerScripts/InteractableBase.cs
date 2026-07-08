using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected bool TutorialPlayed = false;
    public abstract void Interact();
}

public abstract class MinigameInteractable : Interactable
{
    [Header("Camera Information")]
    [SerializeField] private float CameraDelay = 1.0f;
    [SerializeField] private GameObject GameCamera;
    [SerializeField] private GameObject MinigameCamera;
    [SerializeField] protected GameObject X_Object;

    [Header("UI")]
    [SerializeField] protected GameObject UI;
    private void OpenCamera()
    {
        GameCamera.SetActive(false);
        MinigameCamera.SetActive(true);
        UIManager.Instance.RemoveBlackFromScreen(UIManager.Instance.FaderImage.GetComponent<Animator>());
    }

    private void CloseCamera()
    {
        UIManager.Instance.AddBlackToScreen(UIManager.Instance.FaderImage.GetComponent<Animator>());
    }

    public virtual void SetupMinigameLogic()
    {
        UIManager.Instance.ToggleDefaultMinigameUI();
        UI?.SetActive(true);
        UIManager.Instance.CurrentMinigameUI = UI;
    }
    public override void Interact()
    {
        
        
    }

    protected IEnumerator InteractSequence()
    {
        CloseCamera();
        yield return new WaitForSeconds(CameraDelay);
        SetupMinigameLogic();
        yield return new WaitForSeconds(CameraDelay);
        OpenCamera();
        yield return new WaitForSeconds(CameraDelay);
        UIManager.Instance.RemoveFader();
    }

    protected void PutXOnMinigame()
    {
        X_Object.SetActive(true);
    }

    protected void RemoveXFromMinigame()
    {
        X_Object.SetActive(false);
    }
}

