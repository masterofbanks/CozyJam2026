using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SetPlayerChoice(string choice)
    {
        PlayerPrefs.SetString("PlayerType", choice);
        LoadLevel("Gameplay");
    }
}
