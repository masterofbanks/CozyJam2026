using UnityEngine;
using System.Collections.Generic;
using System;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool InMinigame {get; private set;}

    [Header("Mixing/Baking")]
    public string[] RecipesInTray;
    public Tuple<Dictionary<string, int>, int> DrinksContents = new(new(), 0);
    public string DrinkType;
    public bool TrayIsCooked; //{ get; private set; }
    public bool TrayInHand; //{ get; private set; }
    public bool DrinksInHand;
    public bool PlateInHand { get; private set; }
    public string OrderInHand;

    [Header("Player Sprites")]
    [SerializeField] private GameObject TraySprite;
    [SerializeField] private GameObject DrinksSprite;
    [SerializeField] private GameObject PlateSprite;

    [Header("Customer Management")]
    public bool CustomerAtOrderArea { get; private set; } = false;

    public event Action ServedOrderDrinksAction;
    public event Action ServedOrderFoodAction;
    public event Action ServedOrderAction;

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

    

    public bool TrayIsEmpty()
    {
        foreach(string s in RecipesInTray)
        {
            if(!string.IsNullOrEmpty(s))
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
        if (!TrayIsEmpty())
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

    public void PutPlateInHand(string finalOrder)
    {
        PlateInHand = true;
        PlateSprite.SetActive(PlateInHand);
        OrderInHand = finalOrder;
        /*if (TrayIsEmpty())
        {
            TraySprite.SetActive(false);
            TrayIsCooked = false;
            TrayInHand = false;
        }

        if(DrinksContents.Item2 == 0)
        {
            DrinkType = null;
            DrinksInHand = false;
            DrinksSprite.SetActive(false);
        }*/
    }

    public void CleanOutPlateInHand()
    {
        PlateInHand = false;
        PlateSprite.SetActive(PlateInHand);
        OrderInHand = null;
    }

    public void ChangeNumberOfDrinks(int newNumberOfDrinks)
    {
        DrinksContents = new Tuple<Dictionary<string, int>, int>(DrinksContents.Item1, newNumberOfDrinks);
    }

    public void BeginOrderSequence()
    {
        CustomerAtOrderArea = true;
    }

    public void EndOrderSequence()
    {
        CustomerAtOrderArea = false;
    }

    public void CleanUpAfterServing()
    {
        if (TrayIsEmpty())
        {
            TraySprite.SetActive(false);
            TrayIsCooked = false;
            TrayInHand = false;
            ServedOrderFoodAction?.Invoke();
        }

        if (DrinksContents.Item2 == 0)
        {
            DrinkType = null;
            DrinksInHand = false;
            DrinksSprite.SetActive(false);
            ServedOrderDrinksAction?.Invoke();

        }

        ServedOrderAction?.Invoke();
    }


}
