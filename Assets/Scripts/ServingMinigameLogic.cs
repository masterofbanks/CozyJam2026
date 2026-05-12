using System;
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

    }
    private void Start()
    {
    }
    private void OnEnable()
    {
        string[] arrayOfTrayContents = GameManager.Instance.RecipesInTray;
        if (GameManager.Instance.TrayIsCooked)
        {
            for (int i = 0; i < 4; i++)
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
        

        numberOfDrinks = GameManager.Instance.DrinksContents.Item2;
        contentsOfDrink = GameManager.Instance.DrinksContents.Item1;

        UpdateCoffeeContents();
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
        /*string[] arrayOfTrayContents = GameManager.Instance.RecipesInTray;
        if (GameManager.Instance.TrayIsCooked)
        {
            for (int i = 0; i < 4; i++)
            {
                TrayButtons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
                TrayButtons[i].gameObject.SetActive(false);
            }
        }
        

        CoffeeContentsText.text = "";*/
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
            GameManager.Instance.SetFoodInPlateSprite(foodName);
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
            GameManager.Instance.RemoveFoodInPlate();
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
            GameManager.Instance.DrinkSprite.SetActive(true);

        }

        currentDrink = GetDrinkName();
        
    }

    public void PourBackCoffee()
    {
        numberOfDrinks++;
        UpdateCoffeeContents();
        DrinksButton.gameObject.SetActive(false);
        currentDrink = null;
        GameManager.Instance.DrinkSprite.SetActive(false);
    }

    public void ServePlate()
    {
        if(FoodButton.gameObject.activeSelf || DrinksButton.gameObject.activeSelf)
        {
            if (currentTrayIndex != -1 && FoodButton.gameObject.activeSelf)
            {
                GameManager.Instance.RecipesInTray[currentTrayIndex] = null;
                //currentTrayIndex = -1;
            }
            finalOrder = $"{currentDrink}-{currentFood}";
            GameManager.Instance.PutPlateInHand(finalOrder);
            UIManager.Instance.SendBackToCatCamera(Cam);
            if (!GameManager.Instance.FirstCustomerServed)
                SoundManager.PlaySound(SoundType.Walking);

            //DrinksButton.gameObject.SetActive(false);
            //FoodButton.gameObject.SetActive(false);

        }
        else
        {
            GameManager.Instance.CleanOutPlateInHand();
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


