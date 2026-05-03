using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class CoffeeMinigameLogic : MonoBehaviour
{
    public enum CoffeeTypes
    {
        Chocolate,
        Vanilla,
        Colombian,
        French,
        None
    }

    public enum BrewStates
    {
        NotBrewing,
        Brewing,
        Brewed
    }
    [Header("Pot Settings")]
    [SerializeField] private int MaxCapacity = 6;
    [SerializeField] private CoffeeTypes CoffeeType;
    [SerializeField] private BrewStates BrewState;
    private AudioSource coffeePotSource;
    private List<GameObject> _listOfIngredientGameObjects = new();

    [Header("UI")]
    [SerializeField] private RectTransform SpawnPosForItems;
    [SerializeField] private TextMeshProUGUI ContentsText;
    [SerializeField] private GameObject thisCam;

    [Header("Cooking Settings")]
    [SerializeField] private float TimeToMakeCoffee = 2f;
    [SerializeField] private GameObject FinishedSFX;
    private float _t;
    private int _cupsOfWater;
    private Dictionary<string, int> _currentContents;


    private void Awake()
    {
        coffeePotSource = GetComponent<AudioSource>();
        _currentContents = new Dictionary<string, int>();
        _currentContents.Add("Milk", 0);
        _currentContents.Add("Sugar", 0);
        GameManager.Instance.ServedOrderDrinksAction += CleanUpSection;
        
    }

    

    public void ClearContents()
    {
        _currentContents["Milk"] = 0;
        _currentContents["Sugar"] = 0;
        _cupsOfWater = 0;
        BrewState = BrewStates.NotBrewing;
        coffeePotSource.Stop();
        CoffeeType = CoffeeTypes.None;
        ContentsText.text = ConvertContentsToText();
        for(int i = 0; i < _listOfIngredientGameObjects.Count; i++)
        {
            Destroy(_listOfIngredientGameObjects[i]);
        }
        _listOfIngredientGameObjects.Clear();
        


    }

    private string ConvertContentsToText()
    {
        string answer = $"Cups: x{_cupsOfWater}\n";   
        foreach(KeyValuePair<string, int> pair in _currentContents)
        {
            answer += $"{pair.Key}: x{pair.Value}\n";
        }

        return answer;
    }

    private void Start()
    {
        _cupsOfWater = 0;
        BrewState = BrewStates.NotBrewing;
        CoffeeType = CoffeeTypes.None;
    }

    private void Update()
    {
        if(BrewState == BrewStates.Brewing)
        {
            _t += Time.deltaTime;
            if(_t > TimeToMakeCoffee)
            {
                FinishBrewing();
            }
        }
    }

    private void FinishBrewing()
    {
        BrewState = BrewStates.Brewed;
        Instantiate(FinishedSFX);
        coffeePotSource.Stop();
        _t = 0;

    }

    public void AddWater(int amountOfWater)
    {
        if(BrewState == BrewStates.NotBrewing)
        {
            int potentialNewCupsOfWater = _cupsOfWater + amountOfWater;
            if (potentialNewCupsOfWater <= MaxCapacity)
            {
                _cupsOfWater = potentialNewCupsOfWater;
            }
            else
            {
                Debug.Log("Container would overflow!!!");
            }
        }

        ContentsText.text = ConvertContentsToText();
        Debug.Log(_cupsOfWater);
    }

    public void SetCoffeeType(int type)
    {
        if(BrewState == BrewStates.NotBrewing)
        {
            CoffeeType = (CoffeeTypes)type;
        }
    }

    public void StartBrewing()
    {
        if(_cupsOfWater >= 0 && BrewState == BrewStates.NotBrewing && CoffeeType != CoffeeTypes.None)
        {
            BrewState = BrewStates.Brewing;
            coffeePotSource.Play();
        }
    }

    public void AddItem(string name)
    {
        if(BrewState == BrewStates.Brewed)
        {
            if (_currentContents.ContainsKey(name))
            {
                _currentContents[name]++;
                ContentsText.text = ConvertContentsToText();
                Vector3 worldPositionForIngredient = Camera.main.ScreenToWorldPoint(SpawnPosForItems.position);
                worldPositionForIngredient = new Vector3(worldPositionForIngredient.x, worldPositionForIngredient.y, worldPositionForIngredient.z + 10f);
                GameObject item = Instantiate(Resources.Load<GameObject>(name), worldPositionForIngredient, Quaternion.identity);
                _listOfIngredientGameObjects.Add(item);
            }
            else
            {
                Debug.Log($"\"{name}\" Got added man idek check here");
            }
        }
    }

    public void SubmitCoffee()
    {
        if(BrewState == BrewStates.Brewed)
        {
            Dictionary<string, int> copyOfCurrentContents = new();
            foreach(KeyValuePair<string, int> pair in _currentContents)
            {
                copyOfCurrentContents.Add(pair.Key, pair.Value);
            }
            GameManager.Instance.DrinksContents = new Tuple<Dictionary<string, int>, int>(copyOfCurrentContents, _cupsOfWater);
            GameManager.Instance.DrinkType = CoffeeType.ToString();
            BrewState = BrewStates.NotBrewing;
            GameManager.Instance.PutDrinksInHand();
            UIManager.Instance.SendBackToCatCamera(thisCam);
            UIManager.Instance.ToggleDefaultMinigameUI();
        }
    }

    private void CleanUpSection()
    {
        ClearContents();
    }
}
