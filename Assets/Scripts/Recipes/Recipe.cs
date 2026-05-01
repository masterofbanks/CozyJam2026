using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    [Serializable]
    public class Ingredient
    {
        public string name;
        public int amount;
    }

    public List<Ingredient> recipeList;
    public Dictionary<string, int> ConvertListToMap()
    {
        Dictionary<string, int> answer = new();
        foreach(Ingredient ingredient in recipeList)
        {
            answer.TryAdd(ingredient.name, ingredient.amount);
        }

        return answer;
    }
}
