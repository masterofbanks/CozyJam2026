using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ServingMinigameLogic : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private List<Button> TrayButtons;
    [SerializeField] private Button FoodButton;
    [SerializeField] private Button DrinksButton;
    [SerializeField] private TextMeshProUGUI CoffeeContentsText;
    [SerializeField] private GameObject Cam;
    [SerializeField] private Transform ServedPlateParent;
    [SerializeField] private GameObject ServedPlateButtonPrefab;

    private int currentTrayIndex = -1;
    private string currentFood;
    private string currentDrink;
    private string finalOrder;

    private int numberOfDrinks;
    private Dictionary<string, int> contentsOfDrink;

    private void Awake()
    {
        GameManager.Instance.ServedOrderAction += ClearStuffs;
        GameManager.Instance.ServedOrderDrinksAction += ClearDrinks;
        GameManager.Instance.ServedOrderFoodAction += ClearTrayButtons;
        GameManager.Instance.RemovedOrderAction += UpdateServedPlateUI;

    }
    private void Start()
    {
    }
    private void OnEnable()
    {
        string[] arrayOfTrayContents = GameManager.Instance.RecipesInTray;
        if (GameManager.Instance.TrayIsCooked)
        {
            for (int i = 0; i < GameManager.Instance.RecipesInTray.Length; i++)
            {
                if (!string.IsNullOrEmpty(arrayOfTrayContents[i]))
                {
                    TrayButtons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = arrayOfTrayContents[i];
                    TrayButtons[i].gameObject.GetComponent<Image>().sprite = GameManager.Instance.GetFoodImageFromName(arrayOfTrayContents[i]);
                    TrayButtons[i].gameObject.SetActive(true);
                    Debug.Log(arrayOfTrayContents[i]);

                }

                else
                {
                    TrayButtons[i].gameObject.SetActive(false);
                }

            }
        }

        else
        {
            for(int i = 0; i < TrayButtons.Count; i++)
            {
                TrayButtons[i].gameObject.SetActive(false);
            }
        }


            numberOfDrinks = GameManager.Instance.DrinksContents.Item2;
        contentsOfDrink = GameManager.Instance.DrinksContents.Item1;

        UpdateCoffeeContents();
        UpdateServedPlateUI();
    }

    private void UpdateCoffeeContents()
    {
        string contentsOfCoffeeString = $"Cups x{numberOfDrinks}\n";
        contentsOfCoffeeString += GetDrinkName();
        CoffeeContentsText.text = contentsOfCoffeeString;
        GameManager.Instance.ChangeNumberOfDrinks(numberOfDrinks);
    }

    private string GetDrinkName()
    {
        string answer = $"Type: {GameManager.Instance.DrinkType}\n";
        foreach (KeyValuePair<string, int> item in contentsOfDrink)
        {
            answer += $"{item.Key}: x{item.Value}\n";
        }

        return answer;
    }

    private void OnDisable()
    {
       
    }

    public void SubmitFoodItem()
    {
        if (!FoodButton.gameObject.activeSelf)
        {
            int indexOfButton = -1;
            if (!Int32.TryParse(EventSystem.current.currentSelectedGameObject.name, out indexOfButton))
            {
                Debug.Log("LOOK HERE TO FIND OUT OF INDEX ISSUE FOR INDEX OF BUTTON!!");
            }
            currentTrayIndex = indexOfButton;
            string foodName = TrayButtons[currentTrayIndex].gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
            FoodButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = foodName;
            FoodButton.gameObject.GetComponent<Image>().sprite = GameManager.Instance.GetFoodImageFromName(foodName);
            FoodButton.gameObject.SetActive(true);
            currentFood = foodName;
            TrayButtons[indexOfButton].gameObject.SetActive(false);
        }
    }

    public void RemoveFoodItem()
    {
        if (FoodButton.gameObject.activeSelf)
        {
            TrayButtons[currentTrayIndex].gameObject.SetActive(true);
            FoodButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Empty";
            FoodButton.gameObject.SetActive(false);
            currentFood = null;
        }
    }

    public void PourCoffee()
    {
        if (numberOfDrinks > 0 && !DrinksButton.gameObject.activeSelf)
        {
            numberOfDrinks--;
            UpdateCoffeeContents();
            DrinksButton.gameObject.SetActive(true);

        }

        currentDrink = GetDrinkName();
        
    }

    public void PourBackCoffee()
    {
        numberOfDrinks++;
        UpdateCoffeeContents();
        DrinksButton.gameObject.SetActive(false);
        currentDrink = null;
    }

    /// <summary>
    /// This method is called by the Serve Button within the minigame UI. 
    /// If we have a food item and drink item active within the current plate, we add the plate to the inventory of the player and display that plate within the served items UI Grid Layout Group
    /// </summary>
    public void ServePlate()
    {
        if (!SoundManager.TutorialIsPlaying())
        {
            //check if there is a food item and drink item on the plate
            if (FoodButton.gameObject.activeSelf && DrinksButton.gameObject.activeSelf)
            {
                //on serving, remove the food item from the tray and remove both the food item and the drinks item from the plate
                if (currentTrayIndex != -1 && FoodButton.gameObject.activeSelf)
                {
                    GameManager.Instance.RecipesInTray[currentTrayIndex] = null;
                    FoodButton.gameObject.SetActive(false);
                    DrinksButton.gameObject.SetActive(false);
                    //currentTrayIndex = -1;
                }
                GameManager.Instance.AddPlateInHand(currentFood, currentDrink);

                //Update the plate to the served items UI grid layout group
                UpdateServedPlateUI();
                GameManager.Instance.ClearFoodValues();
                GameManager.Instance.ClearDrinkValues();

                if (!GameManager.Instance.FirstCustomerServed)
                {
                    SoundManager.PlaySound(SoundType.Walking);

                }


                

            }
            
        }
        
    }

    /// <summary>
    /// Look at the current inventory of served plates within the game manager, and display their information to the served items UI grid layout group. 
    /// </summary>
    private void UpdateServedPlateUI()
    {
        //clear out old served plate UI
        foreach(Transform child in ServedPlateParent)
        {
            Destroy(child.gameObject);
        }

        //add in a new served plate button for each plate in the inventory of plates in the game manager;
        for(int i = 0; i < GameManager.Instance.OrdersInHand.Count; i++)
        {
            Tuple<string, string> order = GameManager.Instance.OrdersInHand[i];
            GameObject newPlateButton = Instantiate(ServedPlateButtonPrefab, ServedPlateParent);
            newPlateButton.GetComponent<PlateBehavior>().SetUpImagesWithinPlate(order.Item1, order.Item2, i);
        }

    }

    
    public void LeaveMinigame()
    {
        if (!SoundManager.TutorialIsPlaying())
        {
            UIManager.Instance.SendBackToCatCamera(Cam);
            FoodButton.gameObject.SetActive(false);

        }
    }

    

    public void ClearStuffs()
    {
        Debug.Log("Trying to clear stuff within serving minigame logic");
        FoodButton.gameObject.SetActive(false);
        DrinksButton.gameObject.SetActive(false);
    }

    public void ClearDrinks()
    {
        contentsOfDrink.Clear();
        CoffeeContentsText.text = "";
    }

    public void ClearTrayButtons()
    {
        for (int i = 0; i < TrayButtons.Count; i++)
        {
            TrayButtons[i].gameObject.SetActive(false);
        }
    }

    

}


