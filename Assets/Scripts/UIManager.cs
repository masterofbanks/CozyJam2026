using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Components")]
    [SerializeField] private GameObject FaderImage;
    [SerializeField] private GameObject DefaultMinigameUI;

    [Header("Cameras")]
    [SerializeField] private GameObject CatCam;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddBlackToScreen()
    {
        FaderImage.SetActive(true);
        FaderImage.GetComponent<Animator>().Play("MakeBlack");
    }

    public void RemoveBlackFromScreen()
    {
        FaderImage.GetComponent<Animator>().Play("RemoveBlack");
    }

    public void ToggleDefaultMinigameUI()
    {
        DefaultMinigameUI.SetActive(!DefaultMinigameUI.activeSelf);
    }

    public void SendBackToCatCamera(GameObject otherCam)
    {
        StartCoroutine(BringBackToNormalCamSequence(otherCam));
    }

    IEnumerator BringBackToNormalCamSequence(GameObject otherCam)
    {
        AddBlackToScreen();
        yield return new WaitForSeconds(1.0f);
        CatCam.SetActive(true);
        otherCam.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        RemoveBlackFromScreen();
        GameManager.Instance.BringOutOfMinigame();
        yield return new WaitForSeconds(1.0f);
        FaderImage.SetActive(false);

    }

    public void RemoveFader()
    {
        FaderImage?.SetActive(false);
    }
}
