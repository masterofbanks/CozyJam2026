using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Components")]
    [SerializeField] public GameObject FaderImage;
    [SerializeField] private GameObject DefaultMinigameUI;
    [SerializeField] private Animator OrderAnimController;
    [SerializeField] private GameObject OrderSlipPrefab;
    [SerializeField] private Transform SlipParent;
    public GameObject CurrentMinigameUI;
    //
    [Header("Final UI Componets")]
    [SerializeField] private GameObject FinalFaderImage;
    [SerializeField] private GameObject FinalBacgkroundImage;
    [SerializeField] private TextMeshProUGUI FinalScoreText;
    private bool _orderingUIIsUp = false;
    public Dictionary<int, GameObject> MapOfOrders = new();

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

    public void AddBlackToScreen(Animator anime)
    {
        FaderImage.SetActive(true);
        anime.Play("MakeBlack");
    }

    public void RemoveBlackFromScreen(Animator anime)
    {
        anime.Play("RemoveBlack");
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
        AddBlackToScreen(FaderImage.GetComponent<Animator>());
        yield return new WaitForSeconds(1.0f);
        CatCam.SetActive(true);
        otherCam.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        RemoveBlackFromScreen(FaderImage.GetComponent<Animator>());
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
        if (!Int32.TryParse(sugarNumAsString, out sugarNum))
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
        MapOfOrders.Add(id, slip);
    }

    public void RemoveOrderSlip(int id)
    {
        if (MapOfOrders.ContainsKey(id))
        {
            GameObject slip = MapOfOrders[id];
            Destroy(slip);
            MapOfOrders.Remove(id);
        }

        else
        {
            Debug.Log($"COULD NOT FIND ORDER WITH ID {id}");
        }
        
    }

    public IEnumerator LoadLevel(float scoreAsPercent)
    {
        FinalFaderImage.SetActive(true);
        AddBlackToScreen(FinalFaderImage.GetComponent<Animator>());
        yield return new WaitForSeconds(1.0f);
        FinalBacgkroundImage.SetActive(true);
        FinalScoreText.enabled = true;
        FinalScoreText.text = $"You got a {(int)scoreAsPercent}% on the day!";
        RemoveBlackFromScreen(FinalFaderImage.GetComponent<Animator>());
        yield return new WaitForSeconds(3.0f);
        AddBlackToScreen(FinalFaderImage.GetComponent<Animator>());
        yield return new WaitForSeconds(1.0f);
        FinalBacgkroundImage.SetActive(false);
        FinalScoreText.enabled = false;
        RemoveBlackFromScreen(FinalFaderImage.GetComponent<Animator>());
        yield return new WaitForSeconds(1.0f);
        FinalFaderImage.SetActive(false);
    }


}
