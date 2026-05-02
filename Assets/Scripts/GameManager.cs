using UnityEngine;
using System.Collections.Generic;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool InMinigame {get; private set;}

    [Header("Mixing/Baking")]
    public string[] RecipesInTray;
    public Tuple<Dictionary<string, int>, int> DrinksContents;
    public bool TrayIsCooked; //{ get; private set; }
    public bool TrayInHand; //{ get; private set; }
    public bool DrinksInHand;

    [SerializeField] private GameObject TraySprite;
    [SerializeField] private GameObject DrinksSprite;

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

    private void Start()
    {
        RecipesInTray = new string[4];
        TrayIsCooked = false;
    }

    

    public bool TrayIsEmpty(string[] arr)
    {
        foreach(string s in arr)
        {
            if(s != "")
            {
                return false;
            }
        }
        return true;
    }

    public void ThrowTrayInOven()
    {
        TrayInHand = false;
        TraySprite.SetActive(false);
    }
    public void PushIntoMinigame()
    {
        InMinigame = true;
    }

    public void BringOutOfMinigame()
    {
        InMinigame = false;
        
    }

    public void PutTrayInHand()
    {
        if (!TrayIsEmpty(RecipesInTray))
        {
            TrayInHand = true;
            TraySprite.SetActive(TrayInHand);
        }

        else
        {
            Debug.Log("Tray is Empty!!!");
        }
        
    }

    public void PutDrinksInHand()
    {
        DrinksInHand = true;
        DrinksSprite.SetActive(DrinksInHand);
    }




}
