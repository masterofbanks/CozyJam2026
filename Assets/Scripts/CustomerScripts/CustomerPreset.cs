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
    public List<DialogueSequence> DialogueTree = new();
    public int _currentDayIndex = 0;

    public string GetOrder()
    {
        CoffeeOrder order = ScriptableObject.CreateInstance<CoffeeOrder>();
        order.Type = CoffeeType;
        order.NumSugar = NumSugar;
        order.NumMilk = NumMilk;
        return order.ToString() + $"-{FoodType}";
    }

    public DialogueSequence GetCurrentDayOfDialogue()
    {
        return DialogueTree[_currentDayIndex];
    }

    public void ProceedToNextDayOfDialogue()
    {
        if(_currentDayIndex < DialogueTree.Count - 1)
        {
            _currentDayIndex++;
        }

    }

    
}
