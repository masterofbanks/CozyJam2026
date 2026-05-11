using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimations", menuName = "Scriptable Objects/CharacterAnimations")]
public class CharacterAnimations : ScriptableObject
{
    public List<RuntimeAnimatorController> characterAnimationControllers;
    public RuntimeAnimatorController GetRandomCharacterController()
    {
        System.Random r = new System.Random();  
        int randIndex = r.Next(0, characterAnimationControllers.Count);
        return characterAnimationControllers[randIndex];
    }
}
