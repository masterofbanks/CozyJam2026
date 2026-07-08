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

    public string GetOrder()
    {
        CoffeeOrder order = ScriptableObject.CreateInstance<CoffeeOrder>();
        order.Type = CoffeeType;
        order.NumSugar = NumSugar;
        order.NumMilk = NumMilk;
        return order.ToString() + $"-{FoodType}";
    }
}
