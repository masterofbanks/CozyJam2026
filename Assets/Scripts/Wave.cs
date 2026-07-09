using UnityEngine;
using System.Collections.Generic;
using System;
[CreateAssetMenu(fileName = "Wave", menuName = "Scriptable Objects/Wave")]
public class Wave : ScriptableObject
{
    [Serializable]
    public class CustomerType
    {
        public int TimeOfAppearance;
        public bool IsPreset;
        public bool IsRushWave;
    }

    public List<CustomerType> waves = new List<CustomerType>();
}
