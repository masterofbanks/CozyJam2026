using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool InMinigame {get; private set;}
    public bool TrayInHand { get; private set; }

    [Header("Mixing/Baking")]
    public string[] RecipesInTray;
    [SerializeField] private GameObject TraySprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RecipesInTray = new string[4];
    }

    

    public bool TrayIsEmpty(string[] arr)
    {
        foreach(string s in arr)
        {
            if(s != "")
            {
                return false;
            }
        }
        return true;
    }
    public void PushIntoMinigame()
    {
        InMinigame = true;
    }

    public void BringOutOfMinigame()
    {
        InMinigame = false;
        
    }

    public void PutTrayInHand()
    {
        if (!TrayIsEmpty(RecipesInTray))
        {
            TrayInHand = true;
            TraySprite.SetActive(TrayInHand);
        }

        else
        {
            Debug.Log("Tray is Empty!!!");
        }
        
    }
}
