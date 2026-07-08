using System.Globalization;
using UnityEngine;

public class PlateBehavior : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image CoffeeImage;
    [SerializeField] private UnityEngine.UI.Image CoffeeTypeImage;
    [SerializeField] private UnityEngine.UI.Image FoodImage;

    public void SetUpImagesWithinPlate(string drinksOrder, string foodOrder)
    {
        if(foodOrder == null)
        {
            Debug.Log("Food order is null");
            FoodImage.gameObject.SetActive(false);
        }

        else
        {
            FoodImage.sprite = GameManager.Instance.GetFoodImageFromName(foodOrder);
            FoodImage.gameObject.SetActive(true);
        }
        Debug.Log("Hello from the Set up Images within plate method!");
        if(drinksOrder != null)
        {
            CoffeeImage.gameObject.SetActive(true);
            CoffeeTypeImage.sprite = GameManager.Instance.GetDrinkTypeImageFromName(GameManager.Instance.DrinkType);
        }

        else
        {
            CoffeeImage.gameObject.SetActive(false);
        }
    }
}
