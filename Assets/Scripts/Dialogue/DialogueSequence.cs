using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "Scriptable Objects/DialogueSequence")]
public class DialogueSequence : ScriptableObject
{
    [SerializeField] private List<string> SequenceOfLines;

    public string GetDialgoueLine(int index)
    {
        return SequenceOfLines[index];
    }

    public int GetDialgoueSequenceLength()
    {
        return SequenceOfLines.Count;
    }
}
