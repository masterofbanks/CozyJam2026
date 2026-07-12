using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerPreset", menuName = "Scriptable Objects/CustomerPreset")]
public class CustomerPreset : ScriptableObject
{
    public string CustomerName;
    public string CoffeeType;
    public int NumSugar;
    public int NumMilk;
    public string FoodType;
    public int AppearanceIndex;
    public ItemContainer items;

    public string GetOrder()
    {
        CoffeeOrder order = ScriptableObject.CreateInstance<CoffeeOrder>();
        order.Type = CoffeeType;
        order.NumSugar = NumSugar;
        order.NumMilk = NumMilk;
        return order.ToString() + $"-{FoodType}";
    }

    public string CreateOrder()
    {
        CoffeeOrder order = (CoffeeOrder)ScriptableObject.CreateInstance("CoffeeOrder");
        order.Type = items.GetARandomCoffeeOrder();
        order.NumMilk = 0;
        order.NumSugar = 0;
        string drinksOrder = order.ToString();
        string finalOrder = drinksOrder + $"-{items.GetARandomFoodOrder()}";
        return finalOrder;
    }
}
