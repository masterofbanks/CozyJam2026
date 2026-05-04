using UnityEngine;

public class DONTDESTROYONLOAD : MonoBehaviour
{
    public static DONTDESTROYONLOAD Instance { get; private set; } 
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
