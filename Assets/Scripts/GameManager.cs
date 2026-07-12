using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool InMinigame {get; private set;}

    [Header("Wave Settings")]
    [SerializeField] private Day CurrentWave;
    [SerializeField] private int CurrentIndexOfWave = 0;
    [SerializeField] private List<Day> AllWaves;
    [SerializeField] private int ActiveWaveIndex = 0;
    public int NumCustomersServed = 0;
    private float _t;

    [Header("Mixing/Baking")]
    public string[] RecipesInTray;
    [SerializeField] private Sprite NothingSprite;
    public string DrinkType;
    public bool TrayIsCooked; //{ get; private set; }
    public bool TrayInHand; //{ get; private set; }
    public bool TrayInOven;
    public bool DrinksInHand;
    public List<Tuple<string, string>> OrdersInHand = new(); //format is drink order, food order

    public Tuple<Dictionary<string, int>, int> DrinksContents = new(new(), 0);
    private Dictionary<string, Sprite> RecipeImages = new();
    private Dictionary<string, Sprite> DrinkTypes = new();

    [Header("Player Sprites")]
    [SerializeField] private GameObject TraySprite;
    [SerializeField] private GameObject CoffeeMakerSprite;
    [SerializeField] private GameObject PlateImage;
    [SerializeField] private GameObject PlateParentCanvas;


    [Header("Customer Management")]
    public float TotalCustomerScore = 0;
    public bool CustomerAtOrderArea { get; private set; } = false;
    public bool CustomersAreFrozen = false;
    public SoundManager TutorialNoises;
    public bool FirstCustomerServed = false;
    public int RushWaveCustomerCount = 16;
    private List<float> _waveScores = new();

    [Header("Rush Effects")]
    public FullScreenPassRendererFeature DitherShader;


    public event Action ServedOrderDrinksAction;
    public event Action ServedOrderFoodAction;
    public event Action ServedOrderAction;
    public event Action FreezeCustomers;
    public event Action UnfreezeCustomers;
    public event Action StartedNewLevel;
    public event Action BrewedSomeDrinks;
    public event Action MixedSomeFood;
    public event Action RemovedOrderAction;

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

        Sprite[] drinkImageArray = Resources.LoadAll<Sprite>("DrinkTypeImages");
        if(drinkImageArray == null)
        {
            Debug.Log("Could not find drink images");
        }
        else
        {
            DrinkTypes.Add("French", drinkImageArray[1]);
            DrinkTypes.Add("Colombian", drinkImageArray[0]);
            
        }

        RemoveRushEffects();
    }

    private void Start()
    {
        RecipesInTray = new string[8];
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
            if (_t > CurrentWave.waves[CurrentIndexOfWave].TimeOfAppearance)
            {
                if (CurrentWave.waves[CurrentIndexOfWave].IsRushWave)
                {
                    SpawnRushWave(RushWaveCustomerCount);
                }

                else
                {
                    CustomerManager.Instance.SpawnCustomer(CurrentWave.waves[CurrentIndexOfWave].Customer);
                }
                CurrentIndexOfWave++;
            }
        }
        
    }

    public void SpawnRushWave(int amountOfCustomers)
    {
        ApplyRushEffects();
        StartCoroutine(SpawnRushWaveRoutine(0, 1.0f, amountOfCustomers));
    }
    public IEnumerator SpawnRushWaveRoutine(int currentAmountOfCustomers, float interval, int maxAmountOfCustomers)
    {
        if(currentAmountOfCustomers < maxAmountOfCustomers)
        {
            CustomerManager.Instance.SpawnCustomer(null);
            yield return new WaitForSeconds(interval);
            StartCoroutine(SpawnRushWaveRoutine(currentAmountOfCustomers + 1, interval, maxAmountOfCustomers)); 
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
        TrayInOven = true;
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
            MixedSomeFood?.Invoke();

        }

        else
        {
            Debug.Log("Tray is Empty!!!");
        }

        if (TrayInOven)
        {
            TrayInOven = false;
        }
        
    }

    public void PutDrinksInHand()
    {
        DrinksInHand = true;
        CoffeeMakerSprite.SetActive(DrinksInHand);
        BrewedSomeDrinks?.Invoke(); 
    }

    /// <summary>
    /// Add a new order from the serving minigame to the list of orders in the player's hand. Also add a new plate to the player UI canvas to represent the order. 
    /// </summary>
    /// <param name="foodOrder"></param>
    /// <param name="drinksOrder"></param>
    public void AddPlateInHand(string foodOrder, string drinksOrder)
    {
        OrdersInHand.Add(new Tuple<string, string>(drinksOrder, foodOrder));
        UpdateOverworldPlateUI();
    }


    /// <summary>
    /// Clear out the old Plate UI in the overworld, and re add correct plate UI from the current player inventory of orders
    /// </summary>
    private void UpdateOverworldPlateUI()
    {
        for(int i = 0; i < PlateParentCanvas.transform.childCount; i++)
        {
            Destroy(PlateParentCanvas.transform.GetChild(i).gameObject);
        }

        for(int i = 0; i < OrdersInHand.Count; i++)
        {
            Tuple<string, string> order = OrdersInHand[i];
            GameObject plate = Instantiate(PlateImage, PlateParentCanvas.transform);
            plate.GetComponent<PlateBehavior>().SetUpImagesWithinPlate(order.Item1, order.Item2);
        }
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
            CoffeeMakerSprite.SetActive(false);
            ServedOrderDrinksAction?.Invoke();

        }
    }

    /// <summary>
    /// Peform Clean up operations on the player after serving a customer, such as adding the customers score to the player's, and cleaning up UI for the served orders in hand
    /// </summary>
    /// <param name="score"></param>
    public void GiveFoodToCustomer(float score)
    {
        CleanUpAfterServing();
        TotalCustomerScore += score;
        NumCustomersServed++;

        //release the player from tutorial mode
        if (!FirstCustomerServed)
        {
            FirstCustomerServed = true;
        }

        //remove the order from the inventory and update the UI
        RemoveOrderAtIndex(0);

    }

    public void RemoveOrderAtIndex(int index)
    {
        if(index >= 0 && index < OrdersInHand.Count)
        {
            OrdersInHand.RemoveAt(index);
            UpdateOverworldPlateUI();
            RemovedOrderAction?.Invoke();
        }

        else
        {
            Debug.LogWarning($"{index} is out of range {OrdersInHand.Count}");
        }

    }

    public bool AllCustomersServed()
    {
        return NumCustomersServed >= CurrentWave.NumberOfCustomersInWave();
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
        RemoveRushEffects();
        StartCoroutine(StartSequenceOfNextLevel());


    }

    private IEnumerator StartSequenceOfNextLevel()
    {
        yield return new WaitForSeconds(5.5f);
        if (ActiveWaveIndex < AllWaves.Count)
        {
            StartedNewLevel.Invoke();
            _t = 0;
            NumCustomersServed = 0;
            CurrentIndexOfWave = 0;
            SetWave(ActiveWaveIndex);
        }

        else
        {
            SceneManager.LoadScene("EndScreen");
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
        if(RecipeImages.ContainsKey(name))
        {
            return RecipeImages[name];
        }
        else
        {
            Debug.Log($"Could not find food image for {name}");
            return null;
        }
    }

    public Sprite GetDrinkTypeImageFromName(string name)
    {
        if(DrinkTypes.ContainsKey(name))
        {
            return DrinkTypes[name];
        }
        else
        {
            Debug.Log($"Could not find drink type image for {name}");
            return null;
        }
    }

    public void ApplyRushEffects()
    {
        DitherShader.SetActive(true);
        MusicManager.Instance.PlayRushMusic();
    }

    private void RemoveRushEffects()
    {
        DitherShader.SetActive(false);
        MusicManager.Instance.PlayNormalMusic();
    }

    private void OnDestroy()
    {
        DitherShader.SetActive(false);
    }

}
