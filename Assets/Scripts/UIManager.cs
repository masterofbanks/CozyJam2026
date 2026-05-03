using NUnit.Framework.Constraints;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Components")]
    [SerializeField] private GameObject FaderImage;
    [SerializeField] private GameObject DefaultMinigameUI;
    [SerializeField] private Animator OrderAnimController;
    [SerializeField] private GameObject OrderSlipPrefab;
    [SerializeField] private Transform SlipParent;
    public GameObject CurrentMinigameUI;
    private bool _orderingUIIsUp = false;

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
        CurrentMinigameUI?.SetActive(false);
        CurrentMinigameUI = null;
        yield return new WaitForSeconds(1.0f);
        FaderImage.SetActive(false);
        GameManager.Instance.BringOutOfMinigame();


    }

    public void RemoveFader()
    {
        FaderImage?.SetActive(false);
    }

    public void OrderUIButton()
    {
        if (_orderingUIIsUp)
        {
            OrderAnimController.Play("BringDownOrders");
        }

        else
        {
            OrderAnimController.Play("BringUpOrders");
        }

        _orderingUIIsUp = !_orderingUIIsUp;
    }

    public void AddOrderToUI(string order, int id)
    {
        GameObject slip = Instantiate(OrderSlipPrefab, SlipParent);
        int indexOfDash = order.IndexOf('-');
        string food = order.Substring(indexOfDash + 1);
        int indexOfFirstNewline = order.IndexOf('\n');
        Debug.Log(indexOfFirstNewline);
        string drinkType = order.Substring(6, indexOfFirstNewline - 6);
        int indexOfFirstX = order.IndexOf('x');
        string milkNumAsString = order.Substring(indexOfFirstX + 1,1);
        int milkNum = -1;
        if(!Int32.TryParse(milkNumAsString, out milkNum))
        {
            Debug.Log($"Could not convert {milkNumAsString} to an integer");
        }

        bool hasMilk = milkNum >= 1;

        int indexOfLastX = order.LastIndexOf('x');
        string sugarNumAsString = order.Substring(indexOfLastX + 1, 1);
        int sugarNum = -1;
        if (!Int32.TryParse(milkNumAsString, out sugarNum))
        {
            Debug.Log($"Could not convert {sugarNumAsString} to an integer");
        }

        bool hasSugar = sugarNum >= 1;
        string pretty = $"{id}\nFood: {food}\nDrink: {drinkType} ";
        if(hasSugar || hasMilk)
        {
            pretty += "w/";
            if (hasMilk && hasSugar)
            {
                pretty += "Milk and Sugar";
            }

            else if(hasMilk)
            {
                pretty += "Milk";
            }

            else if (hasSugar)
            {
                pretty += "Sugar";
            }
        }

        slip.GetComponentInChildren<TextMeshProUGUI>().text = pretty;
    }

    
}
