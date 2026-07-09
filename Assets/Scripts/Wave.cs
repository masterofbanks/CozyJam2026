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
    public int NumberOfCustomersInWave()
    {
        int answer = 0;
        for(int i = 0; i < waves.Count; i++)
        {
            if (waves[i].IsRushWave)
            {
                answer += GameManager.Instance.RushWaveCustomerCount;
            }

            else
            {
                answer++;
            }
        }

        return answer;
    }
}
