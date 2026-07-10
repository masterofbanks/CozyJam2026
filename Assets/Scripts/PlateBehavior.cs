using System.Globalization;
using UnityEngine;

public class PlateBehavior : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image CoffeeImage;
    [SerializeField] private UnityEngine.UI.Image CoffeeTypeImage;
    [SerializeField] private UnityEngine.UI.Image FoodImage;
    public int _ID { get; private set; }

    public void SetUpImagesWithinPlate(string drinksOrder, string foodOrder, int id = -1)
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
            string typeName = GetTypeFromDrinkOrder(drinksOrder);
            CoffeeImage.gameObject.SetActive(true);
            CoffeeTypeImage.sprite = GameManager.Instance.GetDrinkTypeImageFromName(typeName);
        }

        else
        {
            CoffeeImage.gameObject.SetActive(false);
        }

        _ID = id;   
    }

    /// <summary>
    /// search through the drink order and find its type. Format for the drink order is:
    /// "Type: {DrinkType}\n ingredients that are not implemented..."
    /// </summary>
    /// <param name="drinkOrder"></param>
    /// <returns></returns>
    private string GetTypeFromDrinkOrder(string drinkOrder)
    {
        int indexOfSemiColon = drinkOrder.IndexOf(':');
        int startOfTypeName = indexOfSemiColon + 2;
        int indexOfFirstLineChange = drinkOrder.IndexOf('\n');
        string typeName = drinkOrder.Substring(startOfTypeName, indexOfFirstLineChange - startOfTypeName);
        return typeName;
    }

    /// <summary>
    /// This method is called when a served Button is clicked within the serving minigame.
    /// Removes the served plate assoicated with the index from the inventory, and Updates the served plate UI to display the fact, should also update the overworld served plate UI
    /// </summary>
    /// <param name="index"></param>
    public void RemoveServedOrderFromInventory()
    {
        GameManager.Instance.RemoveOrderAtIndex(this._ID);
    }
}
