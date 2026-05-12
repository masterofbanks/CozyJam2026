using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.UI;

public class MixingMinigameLogic : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform spawnPosForIngredients;
    [SerializeField] private TextMeshProUGUI CurrentRecipeList;

    [Header("Recipes")]
    [SerializeField] private List<Recipe> _recipeList;
    [SerializeField] private List<GameObject> _cannisters; 
    private Dictionary<string, int> _currentItemRecipe;
    private List<GameObject> _currentIngredientList;
    private Dictionary<string,Dictionary<string, int>> _allRecipes;


    private void Awake()
    {
        GameManager.Instance.ServedOrderFoodAction += CleanUpStation;
    }
    void Start()
    {
        _currentItemRecipe = new Dictionary<string, int>();
        _currentIngredientList = new List<GameObject>();
        _allRecipes = new();
        CurrentRecipeList.text = "";
        InitializeRecipes();
    }

    private void InitializeRecipes()
    {
        foreach(Recipe recipe in _recipeList)
        {
            _allRecipes.Add(recipe.name, recipe.ConvertListToMap());
            Debug.Log(ConvertRecipeSetToString(recipe.ConvertListToMap()));
        }
    }

    public void AddItem(GameObject ingredientPrefab)
    {
        if (ThereAreOpenSlots())
        {
            Vector3 worldPositionForIngredient = Camera.main.ScreenToWorldPoint(spawnPosForIngredients.position);
            worldPositionForIngredient = new Vector3(worldPositionForIngredient.x, worldPositionForIngredient.y, worldPositionForIngredient.z + 10f);
            GameObject item = Instantiate(ingredientPrefab, worldPositionForIngredient, Quaternion.identity);
            _currentIngredientList.Add(item);
            if (_currentItemRecipe.ContainsKey(ingredientPrefab.name))
            {
                _currentItemRecipe[ingredientPrefab.name]++;
            }

            else
            {
                _currentItemRecipe.Add(ingredientPrefab.name, 1);
            }
            Debug.Log(ConvertRecipeSetToString(_currentItemRecipe));
            CurrentRecipeList.text = ConvertRecipeSetToString(_currentItemRecipe);
        }

        else
        {
            Debug.Log("No Slots Avail!");
        }
        
    }


    private string ConvertRecipeSetToString(Dictionary<string, int> set)
    {
        string answer = "";
        foreach (KeyValuePair<string, int> item in set)
        {
            answer += $"{item.Key}: x{item.Value}\n";
        }

        return answer;
    }

    public void ClearCurrentRecipe()
    {
        _currentItemRecipe.Clear();
        CurrentRecipeList.text = "";
        foreach(GameObject obj in _currentIngredientList)
        {
            Destroy(obj);
        }
        _currentIngredientList.Clear();

    }

    public void SubmitRecipe()
    {
        if(_currentIngredientList.Count > 0)
        {
            if (ThereAreOpenSlots())
            {
                foreach (KeyValuePair<string, Dictionary<string, int>> recipe in _allRecipes)
                {
                    /*// Source - https://stackoverflow.com/a/3804852
                        Posted by Nick Jones, modified by community. See post 'Timeline' for change history
                        Retrieved 2026-05-01, License - CC BY-SA 2.5

                        dic1.Count == dic2.Count && !dic1.Except(dic2).Any();

                     * 
                     * 
                     * */
                    if (recipe.Value.Count == _currentItemRecipe.Count && !recipe.Value.Except(_currentItemRecipe).Any())
                    {
                        Debug.Log(recipe.Key);
                        AddItemToTray(recipe.Key);
                        return;
                    }
                }
                ClearCurrentRecipe();
                AddItemToTray("Nothing");
                Debug.Log("Nothing");
            }

            else
            {
                Debug.Log("No Slots Avail!");

            }
        }


        else
        {
            Debug.Log("There is nothing in the ingredient list");
        }


    }

    private void AddItemToTray(string name)
    {
        ClearCurrentRecipe();
        int firstOpenSlotInTray = FindFirstOpenSlot();
        if (firstOpenSlotInTray == -1 || firstOpenSlotInTray >= 4)
        {
            Debug.Log("openSlot index out of range look here!!!");
        }
        GameManager.Instance.RecipesInTray[firstOpenSlotInTray] = name;
        //_cannisters[firstOpenSlotInTray].GetComponentInChildren<TextMeshProUGUI>().text = name;
        _cannisters[firstOpenSlotInTray].GetComponent<Image>().sprite = GameManager.Instance.GetFoodImageFromName(name);
        _cannisters[firstOpenSlotInTray].SetActive(true);
    }
    public bool ThereAreOpenSlots()
    {
        foreach(GameObject slot in _cannisters)
        {
            if (!slot.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    public int FindFirstOpenSlot()
    {
        int answer = 0;
        foreach(GameObject slot in _cannisters)
        {
            if (!slot.activeSelf)
            {
                return answer;
            }

            answer++;
        }

        return -1;
    }

    public void ClearTray()
    {
        for(int i = 0; i < _cannisters.Count; i++)
        {
            ClearItemButton(i);
        }
    }

    public void ClearItemInTray(GameObject item)
    {
        item.GetComponentInChildren<TextMeshProUGUI>().text = "";
        item.SetActive(false);
    }

    public void ClearItemButton(int i)
    {
        ClearItemInTray(_cannisters[i]);
        GameManager.Instance.RecipesInTray[i] = "";
    }

    public void MoveToOven()
    {
        if (!SoundManager.TutorialIsPlaying())
        {
            GameManager.Instance.PutTrayInHand();
            SoundManager.PlaySound(SoundType.Oven);

        }
    }

    public void CleanUpStation()
    {
        ClearCurrentRecipe();
        ClearTray();
    }
}
