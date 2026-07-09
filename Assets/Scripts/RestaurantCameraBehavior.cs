using UnityEngine;

public class RestaurantCameraBehavior : MonoBehaviour
{
    public GameObject Camera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Camera.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Camera.SetActive(false);
        }
    }
}
