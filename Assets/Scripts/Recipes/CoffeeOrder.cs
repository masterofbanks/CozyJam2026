using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CoffeeOrder", menuName = "Scriptable Objects/CoffeeOrder")]
[System.Serializable]
public class CoffeeOrder : ScriptableObject
{
    public string Type;
    public int NumSugar;
    public int NumMilk;

    public override string ToString()
    {
        return $"Type: {Type}\nMilk: x{NumMilk}\nSugar: x{NumSugar}\n";
    }

}
