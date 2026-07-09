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
    public bool TrayInOven;
    public bool DrinksInHand;
    public bool PlateInHand { get; private set; }
    public List<string> OrdersInHand;

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
    public FullScreenPassRendererFeature DitherShader;
    public SoundManager TutorialNoises;
    public bool FirstCustomerServed = false;
    [SerializeField] private int RushWaveCustomerCount = 16;
    private List<float> _waveScores = new();

    public event Action ServedOrderDrinksAction;
    public event Action ServedOrderFoodAction;
    public event Action ServedOrderAction;
    public event Action FreezeCustomers;
    public event Action UnfreezeCustomers;
    public event Action StartedNewLevel;
    public event Action BrewedSomeDrinks;
    public event Action MixedSomeFood;

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
                    CustomerManager.Instance.SpawnCustomer(CurrentWave.waves[CurrentIndexOfWave].IsPreset);
                }
                CurrentIndexOfWave++;
            }
        }
        
    }

    public void SpawnRushWave(int amountOfCustomers)
    {
        StartCoroutine(SpawnRushWaveRoutine(0, 1.0f, amountOfCustomers));
    }
    public IEnumerator SpawnRushWaveRoutine(int currentAmountOfCustomers, float interval, int maxAmountOfCustomers)
    {
        if(currentAmountOfCustomers < maxAmountOfCustomers)
        {
            CustomerManager.Instance.SpawnCustomer();
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

    public void AddPlateInHand(string foodOrder, string drinksOrder)
    {
        PlateInHand = true;
        GameObject plate = Instantiate(PlateImage, PlateParentCanvas.transform);
        string finalOrder = $"{drinksOrder}-{foodOrder}";
        OrdersInHand.Add(finalOrder);
        plate.GetComponent<PlateBehavior>().SetUpImagesWithinPlate(drinksOrder, foodOrder);
    }

   
    public void CleanOutPlateInHand()
    {
        PlateInHand = false;
        //PlateSprite.SetActive(PlateInHand);
        //OrderInHand = null;
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

    public void GiveFood(float score)
    {
        CleanOutPlateInHand();
        CleanUpAfterServing();
        TotalCustomerScore += score;
        NumCustomersServed++;
        //DrinkSprite.SetActive(false);
        //FoodSprite.SetActive(false);
        if (!FirstCustomerServed)
        {
            FirstCustomerServed = true;
        }
        OrdersInHand.RemoveAt(0);   
        Destroy(PlateParentCanvas.transform.GetChild(0).gameObject);

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

    public void SetFoodInPlateSprite(string name)
    {
        //FoodSprite.gameObject.SetActive(true);
       // FoodSprite.GetComponent<SpriteRenderer>().sprite = GetFoodImageFromName(name);
    }

    public void RemoveFoodInPlate()
    {
        //FoodSprite.gameObject?.SetActive(false);
        //GetComponent<SpriteRenderer>().sprite = null;
    }

   
}
