using UnityEngine;
using System.Collections.Generic;
using System;
[CreateAssetMenu(fileName = "Day", menuName = "Scriptable Objects/Day")]
public class Day : ScriptableObject
{
    [Serializable]
    public class WaveType
    {
        public int TimeOfAppearance;
        public CustomerPreset Customer;
        public bool IsRushWave;
    }

    public List<WaveType> waves = new List<WaveType>();
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
