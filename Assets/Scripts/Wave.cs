using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Wave", menuName = "Scriptable Objects/Wave")]
public class Wave : ScriptableObject
{
    public List<int> waves = new List<int>();
}
