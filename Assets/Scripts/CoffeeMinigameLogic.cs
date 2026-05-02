using UnityEngine;

public class CoffeeMinigameLogic : MonoBehaviour
{
    public enum CoffeeTypes
    {
        Chocolate,
        Vanilla,
        Colombian,
        French
    }
    [Header("Pot Settings")]
    [SerializeField] private int MaxCapacity = 6;
    [SerializeField] private CoffeeTypes CoffeeType;
    private int _cupsOfWater;

    private void Start()
    {
        _cupsOfWater = 0;
    }

    public void AddWater(int amountOfWater)
    {
        int potentialNewCupsOfWater = _cupsOfWater + amountOfWater;
        if(potentialNewCupsOfWater <= MaxCapacity)
        {
            _cupsOfWater = potentialNewCupsOfWater;
        }
        else
        {
            Debug.Log("Container would overflow!!!");
        }

        Debug.Log(_cupsOfWater);
    }

    public void SetCoffeeType(int type)
    {
        CoffeeType = (CoffeeTypes)type;
    }
}
