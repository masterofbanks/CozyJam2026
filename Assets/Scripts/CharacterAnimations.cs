using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimations", menuName = "Scriptable Objects/CharacterAnimations")]
public class CharacterAnimations : ScriptableObject
{
    public List<RuntimeAnimatorController> characterAnimationControllers;
    public List<Sprite> EntitySprites;
    public RuntimeAnimatorController GetRandomCharacterController(out int indexOfCharacterAnimator)
    {
        System.Random r = new System.Random();  
        int randIndex = r.Next(0, characterAnimationControllers.Count);
        indexOfCharacterAnimator = randIndex;
        return characterAnimationControllers[randIndex];
    }

    public RuntimeAnimatorController GetCharacterController(int index)
    {
        return characterAnimationControllers[index];
    }

    public Dictionary<string, RuntimeAnimatorController> ConvertListToDictionary()
    {
        Dictionary<string, RuntimeAnimatorController> answer = new();
        foreach(var controller in characterAnimationControllers)
        {
            answer.Add(controller.name, controller);
        }
        return answer;
    }
}
