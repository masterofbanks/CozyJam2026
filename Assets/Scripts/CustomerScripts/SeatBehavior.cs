using UnityEngine;

public class SeatBehavior : MonoBehaviour
{
    public bool Occupied { get; private set; }
    public void OccupySeat()
    {
        Occupied = true;
    }

    
}
