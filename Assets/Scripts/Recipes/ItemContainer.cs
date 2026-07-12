using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemContainer", menuName = "Scriptable Objects/ItemContainer")]
public class ItemContainer : ScriptableObject
{
    public List<CoffeeOrder> CoffeeOrderTypes;
    public List<Recipe> FoodOrderTypes;

    public string GetARandomCoffeeOrder()
    {
        System.Random r = new System.Random();
        int randIndex = r.Next(0, CoffeeOrderTypes.Count);
        return CoffeeOrderTypes[randIndex].Type;
    }

    public string GetARandomFoodOrder()
    {
        System.Random r = new System.Random();
        int randIndex = r.Next(0, FoodOrderTypes.Count);
        return FoodOrderTypes[randIndex].name;
    }
}
