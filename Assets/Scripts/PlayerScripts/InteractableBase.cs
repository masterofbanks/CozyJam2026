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
    private void OpenCamera()
    {

    }

    private void CloseCamera()
    {

    }

    public abstract void SetupMinigameLogic();
    public override void Interact()
    {
        StartCoroutine(InteractSequence());
    }

    private IEnumerator InteractSequence()
    {
        CloseCamera();
        yield return new WaitForSeconds(CameraDelay);
        SetupMinigameLogic();
        yield return new WaitForSeconds(CameraDelay);
        OpenCamera();
    }
}

