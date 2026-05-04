using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool InMinigame {get; private set;}

    [Header("Wave Settings")]
    [SerializeField] private Wave CurrentWave;
    [SerializeField] private int CurrentIndexOfWave = 0;
    public int NumCustomersServed = 0;
    public string NextLevelName;
    private float _t;

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
    public float TotalCustomerScore = 0;
    public bool CustomerAtOrderArea { get; private set; } = false;
    public bool CustomersAreFrozen = false;
    public FullScreenPassRendererFeature DitherShader;
    public SoundManager TutorialNoises;
    public bool FirstCustomerServed = false;

    public event Action ServedOrderDrinksAction;
    public event Action ServedOrderFoodAction;
    public event Action ServedOrderAction;
    public event Action FreezeCustomers;
    public event Action UnfreezeCustomers;

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
        if(TutorialNoises != null)
        {
            Debug.Log("Hello from intro sound!");
            StartCoroutine(SmallDelay());
        }
    }

    IEnumerator SmallDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SoundManager.PlaySound(SoundType.Intro);

    }

    private void Update()
    {
        if (CurrentIndexOfWave < CurrentWave.waves.Count)
        {
            _t += Time.deltaTime;
            if (_t > CurrentWave.waves[CurrentIndexOfWave])
            {
                CurrentIndexOfWave++;
                CustomerManager.Instance.SpawnCustomer();
            }
        }
        
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
        CustomerAtOrderArea = CustomerManager.Instance.TestForCustomersInLine();
    }

    public void ClearTrayArray()
    {
        for (int i = 0; i < 4; i++)
        {
            RecipesInTray[i] = null;
        }
    }

    public void ClearDrinksOut()
    {
        DrinksContents = new(new(), 0);
    }

    public void CleanUpAfterServing()
    {
        ClearFoodValues();
        ClearDrinkValues();

        

        ServedOrderAction?.Invoke();
    }

    public void ClearFoodValues()
    {
        if (TrayIsEmpty())
        {
            TraySprite.SetActive(false);
            TrayIsCooked = false;
            TrayInHand = false;
            ServedOrderFoodAction?.Invoke();
        }
    }

    public void ClearDrinkValues()
    {
        if (DrinksContents.Item2 == 0)
        {
            DrinkType = null;
            DrinksInHand = false;
            DrinksSprite.SetActive(false);
            ServedOrderDrinksAction?.Invoke();

        }
    }

    public void GiveFood(float score)
    {
        CleanOutPlateInHand();
        CleanUpAfterServing();
        GameManager.Instance.TotalCustomerScore += score;
        NumCustomersServed++;
        if (!FirstCustomerServed)
        {
            if(TutorialNoises != null)
            {
                SoundManager.PlaySound(SoundType.Extra);
            }
            FirstCustomerServed = true;

        }
    }

    public bool AllCustomersServed()
    {
        return NumCustomersServed >= CurrentWave.waves.Count;
    }

    public void LoadNextLevel()
    {
        if (AllCustomersServed())
        {
            float score = TotalCustomerScore * 100f / (CurrentWave.waves.Count * 80f);
            StartCoroutine(UIManager.Instance.LoadLevel(NextLevelName, score));
        }
    }

    

    public void MakeCustomersFrozen()
    {
        if (!CustomersAreFrozen)
        {
            FreezeCustomers?.Invoke();
        }

        else
        {
            UnfreezeCustomers?.Invoke();
        }
        DitherShader.SetActive(!CustomersAreFrozen);
        CustomersAreFrozen = !CustomersAreFrozen;


    }


}
