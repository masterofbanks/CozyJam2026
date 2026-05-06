using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool InMinigame {get; private set;}

    [Header("Wave Settings")]
    [SerializeField] private Wave CurrentWave;
    [SerializeField] private int CurrentIndexOfWave = 0;
    [SerializeField] private List<Wave> AllWaves;
    [SerializeField] private int ActiveWaveIndex = 0;
    public int NumCustomersServed = 0;
    private float _t;

    [Header("Mixing/Baking")]
    public string[] RecipesInTray;
    [SerializeField] private Sprite NothingSprite;
    public string DrinkType;
    public bool TrayIsCooked; //{ get; private set; }
    public bool TrayInHand; //{ get; private set; }
    public bool DrinksInHand;
    public bool PlateInHand { get; private set; }
    public string OrderInHand;

    public Tuple<Dictionary<string, int>, int> DrinksContents = new(new(), 0);
    private Dictionary<string, Sprite> RecipeImages = new();

    [Header("Player Sprites")]
    [SerializeField] private GameObject TraySprite;
    [SerializeField] private GameObject DrinksSprite;
    [SerializeField] private GameObject PlateSprite;
    [SerializeField] public GameObject DrinkSprite;
    [SerializeField] public GameObject FoodSprite;


    [Header("Customer Management")]
    public float TotalCustomerScore = 0;
    public bool CustomerAtOrderArea { get; private set; } = false;
    public bool CustomersAreFrozen = false;
    public FullScreenPassRendererFeature DitherShader;
    public SoundManager TutorialNoises;
    public bool FirstCustomerServed = false;
    private List<float> _waveScores = new();

    public event Action ServedOrderDrinksAction;
    public event Action ServedOrderFoodAction;
    public event Action ServedOrderAction;
    public event Action FreezeCustomers;
    public event Action UnfreezeCustomers;
    public event Action StartedNewLevel;

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
        CurrentIndexOfWave = 0;
        ActiveWaveIndex = 0;
        SetWave(ActiveWaveIndex);
        Sprite[] foodImageArray = Resources.LoadAll<Sprite>("neko-cafe-food");
        if(foodImageArray == null)
        {
            Debug.Log("Could not find food images");
        }
        else
        {
            RecipeImages.Add("Cake", foodImageArray[0]);
            RecipeImages.Add("Strudel", foodImageArray[2]);
            RecipeImages.Add("Pie", foodImageArray[3]);
            RecipeImages.Add("Croissant", foodImageArray[4]);
            RecipeImages.Add("Toast", foodImageArray[5]);
            RecipeImages.Add("Nothing", NothingSprite);

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
        if (!FirstCustomerServed)
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
        TotalCustomerScore += score;
        NumCustomersServed++;
        DrinkSprite.SetActive(false);
        FoodSprite.SetActive(false);
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
            _waveScores.Add(score);
            TotalCustomerScore = 0;
            StartCoroutine(UIManager.Instance.LoadLevel(score));
            ClearTrayArray();
            ClearFoodValues();
            ClearDrinksOut();
            ClearDrinkValues();
            StartNextLevel();
        }
    }

    public void StartNextLevel() 
    {
        ActiveWaveIndex++;
        if(ActiveWaveIndex < AllWaves.Count)
        {
            StartCoroutine(StartSequenceOfNextLevel());
        }

        else
        {
            Debug.Log("Run out of waves to spawn!");
        }


    }

    private IEnumerator StartSequenceOfNextLevel()
    {
        yield return new WaitForSeconds(5.5f);
        StartedNewLevel.Invoke();
        _t = 0;
        NumCustomersServed = 0;
        CurrentIndexOfWave = 0;
        SetWave(ActiveWaveIndex);
        
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

    private void SetWave(int index)
    {
        CurrentWave = AllWaves[index];
        if (CurrentWave == null)
        {
            Debug.Log("No Current Wave Selected;");
        }
    }

    public Sprite GetFoodImageFromName(string name)
    {
        return RecipeImages[name];
    }

    public void SetFoodInPlateSprite(string name)
    {
        FoodSprite.gameObject.SetActive(true);
        FoodSprite.GetComponent<SpriteRenderer>().sprite = GetFoodImageFromName(name);
    }
}
