using UnityEngine;

public class SeatBehavior : MonoBehaviour
{
    public bool Occupied;
    public void OccupySeat()
    {
        Occupied = true;
    }

    public void DeoccupySeat()
    {
        Occupied = false;
    }

    
}
